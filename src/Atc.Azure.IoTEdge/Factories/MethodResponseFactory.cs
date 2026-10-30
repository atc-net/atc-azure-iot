namespace Atc.Azure.IoTEdge.Factories;

/// <summary>
/// Creates direct method responses with the data serialized as JSON.
/// </summary>
/// <remarks>
/// The default serializer writes camelCase property names and no indentation, because a direct method response is
/// limited in size (128 KB).
/// </remarks>
public sealed class MethodResponseFactory : IMethodResponseFactory
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public MethodResponseFactory()
        => jsonSerializerOptions = JsonSerializerOptionsFactory.Create(
            new JsonSerializerFactorySettings
            {
                WriteIndented = false,
            });

    public MethodResponseFactory(
        JsonSerializerOptions jsonSerializerOptions)
        => this.jsonSerializerOptions = jsonSerializerOptions;

    public MethodResponse Create(
        HttpStatusCode statusCode,
        object data)
        => new(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, jsonSerializerOptions)),
            (int)statusCode);
}