namespace Atc.Azure.Iot.Sample.EventViewer;

/// <summary>
/// Reads the Event Hubs from their first event on and keeps the latest events, newest first.
/// </summary>
public sealed class EventFeed
{
    private const int MaxEvents = 1000;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    private readonly Lock gate = new();
    private readonly List<ForwardedEvent> events = [];
    private readonly Dictionary<string, int> counts = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> errors = new(StringComparer.Ordinal);

    /// <summary>
    /// Reads an Event Hub until cancelled. After an error, it reads the Event Hub again from its first event.
    /// </summary>
    /// <param name="client">The consumer client of the Event Hub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when cancelled.</returns>
    public async Task ReadAsync(
        EventHubConsumerClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);

        lock (gate)
        {
            counts[client.EventHubName] = 0;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var partitionEvent in client.ReadEventsAsync(startReadingAtEarliestEvent: true, cancellationToken: cancellationToken))
                {
                    Add(ForwardedEvent.From(client.EventHubName, partitionEvent.Data));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (EventHubsException ex)
            {
                // Reading again starts from the first event, so the events read so far are dropped instead of shown twice.
                Fail(client.EventHubName, ex.Message);
                await Task.Delay(RetryDelay, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Gets the latest events, newest first, the count per Event Hub and the read errors.
    /// </summary>
    /// <returns>A copy that later events don't change.</returns>
    public (IReadOnlyList<ForwardedEvent> Events, IReadOnlyList<KeyValuePair<string, int>> Counts, IReadOnlyList<string> Errors) GetSnapshot()
    {
        lock (gate)
        {
            return (
                [.. events],
                [.. counts],
                [.. errors.Select(x => $"{x.Key}: {x.Value}")]);
        }
    }

    private void Add(ForwardedEvent forwardedEvent)
    {
        lock (gate)
        {
            events.Insert(0, forwardedEvent);
            if (events.Count > MaxEvents)
            {
                events.RemoveAt(events.Count - 1);
            }

            counts[forwardedEvent.EventHubName]++;
            errors.Remove(forwardedEvent.EventHubName);
        }
    }

    private void Fail(
        string eventHubName,
        string error)
    {
        lock (gate)
        {
            events.RemoveAll(x => x.EventHubName == eventHubName);
            counts[eventHubName] = 0;
            errors[eventHubName] = $"{error} Reading again in {RetryDelay.TotalSeconds:0} seconds.";
        }
    }
}