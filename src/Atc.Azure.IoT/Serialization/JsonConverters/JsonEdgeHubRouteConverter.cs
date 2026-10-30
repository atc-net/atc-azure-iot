namespace Atc.Azure.IoT.Serialization.JsonConverters;

/// <summary>
/// Reads and writes an edgeHub route with System.Text.Json, in either of the two formats a deployment manifest allows:
/// the route text (<c>"FROM /messages/* INTO $upstream"</c>) or an object with the route, its priority and its time to
/// live (<c>{ "route": "...", "priority": 0, "timeToLiveSecs": 86400 }</c>).
/// </summary>
/// <remarks>
/// <see cref="Converters.EdgeHubRouteConverter"/> does the same for Newtonsoft.Json.
/// </remarks>
public sealed class JsonEdgeHubRouteConverter : JsonConverter<EdgeHubRoute>
{
    public override EdgeHubRoute Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new EdgeHubRoute(reader.GetString()!, Priority: null, TimeToLiveSecs: null);
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"An edgeHub route must be text or an object, but was {reader.TokenType}.");
        }

        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        return new EdgeHubRoute(
            root.TryGetProperty(PropertyNames.Manifest.Route, out var route) ? route.GetString() ?? string.Empty : string.Empty,
            root.TryGetProperty(PropertyNames.Manifest.Priority, out var priority) ? priority.GetInt32() : null,
            root.TryGetProperty(PropertyNames.Manifest.TimeToLiveSecs, out var timeToLive) ? timeToLive.GetInt32() : null);
    }

    public override void Write(
        Utf8JsonWriter writer,
        EdgeHubRoute value,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        if (value.Priority is null &&
            value.TimeToLiveSecs is null)
        {
            writer.WriteStringValue(value.Route);
            return;
        }

        writer.WriteStartObject();
        writer.WriteString(PropertyNames.Manifest.Route, value.Route);

        if (value.Priority.HasValue)
        {
            writer.WriteNumber(PropertyNames.Manifest.Priority, value.Priority.Value);
        }

        if (value.TimeToLiveSecs.HasValue)
        {
            writer.WriteNumber(PropertyNames.Manifest.TimeToLiveSecs, value.TimeToLiveSecs.Value);
        }

        writer.WriteEndObject();
    }
}