namespace Atc.Azure.Iot.Sample.EventViewer;

/// <summary>
/// An event read from one of the Event Hubs that the IoT Hub event bridge forwards to.
/// </summary>
/// <param name="EventHubName">The Event Hub the event was read from.</param>
/// <param name="Time">The time IoT Hub received the event.</param>
/// <param name="DeviceId">The device the event is about, if known.</param>
/// <param name="ModuleId">The module the event is about, if any.</param>
/// <param name="Summary">What happened, e.g. the operation of a lifecycle event or the start of a telemetry message.</param>
/// <param name="Body">The body as text.</param>
/// <param name="ApplicationProperties">The application properties, which consumers read, e.g. <c>opType</c>.</param>
/// <param name="SystemProperties">The system properties, e.g. <c>iothub-message-source</c>.</param>
public sealed record ForwardedEvent(
    string EventHubName,
    DateTimeOffset Time,
    string? DeviceId,
    string? ModuleId,
    string Summary,
    string Body,
    IReadOnlyList<KeyValuePair<string, string>> ApplicationProperties,
    IReadOnlyList<KeyValuePair<string, string>> SystemProperties)
{
    private const int SummaryLength = 60;

    public static ForwardedEvent From(
        string eventHubName,
        EventData eventData)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        var body = eventData.EventBody.ToString();
        var applicationProperties = ToText(eventData.Properties);
        var systemProperties = ToText(eventData.SystemProperties);

        // IoT Hub's own receive time, which the bridge copies; the Event Hub's time is when the bridge forwarded it.
        var time = eventData.SystemProperties.TryGetValue("iothub-enqueuedtime", out var enqueuedTime) && enqueuedTime is DateTime dateTime
            ? new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc))
            : eventData.EnqueuedTime;

        return new ForwardedEvent(
            eventHubName,
            time,
            Find("iothub-connection-device-id", "deviceId"),
            Find("iothub-connection-module-id", "moduleId"),
            applicationProperties.FirstOrDefault(x => x.Key == "opType").Value ?? Shorten(body),
            body,
            applicationProperties,
            systemProperties);

        string? Find(
            string systemProperty,
            string applicationProperty)
            => systemProperties.FirstOrDefault(x => x.Key == systemProperty).Value
               ?? applicationProperties.FirstOrDefault(x => x.Key == applicationProperty).Value;
    }

    private static List<KeyValuePair<string, string>> ToText(IEnumerable<KeyValuePair<string, object>> properties)
        => properties
            .Select(x => new KeyValuePair<string, string>(x.Key, ToText(x.Value)))
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .ToList();

    private static string ToText(object? value)
        => value switch
        {
            null => string.Empty,
            DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),

            // E.g. the user-id of the AMQP properties section, which IoT Hub sets to the hub name.
            ReadOnlyMemory<byte> bytes => Encoding.UTF8.GetString(bytes.Span),
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            IFormattable formattable => formattable.ToString(format: null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty,
        };

    private static string Shorten(string body)
    {
        var line = body.ReplaceLineEndings(" ").Trim();
        return line.Length <= SummaryLength
            ? line
            : string.Concat(line.AsSpan(0, SummaryLength - 1), "…");
    }
}