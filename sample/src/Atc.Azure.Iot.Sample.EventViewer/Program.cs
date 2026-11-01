namespace Atc.Azure.Iot.Sample.EventViewer;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // The screen owns the terminal, so nothing else writes to it.
        builder.Logging.ClearProviders();

        // The AppHost passes a connection string per Event Hub consumer group, named after the consumer group resource.
        // Aspire passes each one twice, also with underscores for dashes in the name, so each is read once.
        var connectionNames = builder.Configuration
            .GetSection("ConnectionStrings")
            .GetChildren()
            .DistinctBy(x => x.Value, StringComparer.Ordinal)
            .Select(x => x.Key)
            .ToList();

        foreach (var connectionName in connectionNames)
        {
            builder.AddKeyedAzureEventHubConsumerClient(connectionName);
        }

        using var host = builder.Build();
        using var stopping = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            stopping.Cancel();
        };

        var feed = new EventFeed();
        var readers = connectionNames
            .Select(x => feed.ReadAsync(host.Services.GetRequiredKeyedService<EventHubConsumerClient>(x), stopping.Token))
            .ToList();

        await new EventViewerScreen(feed).RunAsync(stopping.Token);
        await Task.WhenAll(readers);
    }
}