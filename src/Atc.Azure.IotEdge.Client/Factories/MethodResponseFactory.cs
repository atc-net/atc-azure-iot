namespace Atc.Azure.IotEdge.Client.Factories;

public class MethodResponseFactory : IMethodResponseFactory
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public MethodResponseFactory()
    {
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
    }

    public MethodResponseFactory(
        JsonSerializerOptions jsonSerializerOptions)
    {
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    public MethodResponse Create(
        HttpStatusCode statusCode,
        object data)
        => new(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, jsonSerializerOptions)),
            (int)statusCode);
}