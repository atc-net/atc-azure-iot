namespace Atc.Azure.IoT.Converters;

public sealed class EdgeHubRouteConverter : Newtonsoft.Json.JsonConverter<EdgeHubRoute>
{
    public override bool CanWrite => false;

    public override EdgeHubRoute? ReadJson(
        Newtonsoft.Json.JsonReader reader,
        Type objectType,
        EdgeHubRoute? existingValue,
        bool hasExistingValue,
        Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.TokenType == Newtonsoft.Json.JsonToken.String)
        {
            var expr = (string)reader.Value!;
            return new EdgeHubRoute(expr, null, null);
        }

        var jObject = Newtonsoft.Json.Linq.JObject.Load(reader);

        var routeExpr = jObject[PropertyNames.Manifest.Route]?.ToString() ?? string.Empty;
        var priority = jObject[PropertyNames.Manifest.Priority]?.ToObject<int?>();
        var ttlSecs = jObject[PropertyNames.Manifest.TimeToLiveSecs]?.ToObject<int?>();

        return new EdgeHubRoute(routeExpr, priority, ttlSecs);
    }

    public override void WriteJson(
        Newtonsoft.Json.JsonWriter writer,
        EdgeHubRoute? value,
        Newtonsoft.Json.JsonSerializer serializer)
        => throw new NotSupportedException();
}