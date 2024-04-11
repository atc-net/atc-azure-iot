namespace Atc.Azure.IoT.Models;

public record MethodResultModel(
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("jsonPayload")] string JsonPayload);

public record MethodParameterModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("jsonPayload")] string JsonPayload);

public record MethodResultErrorModel(
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("detail")] string Detail);

public record RestartModuleRequest(
    string Id,
    string SchemaVersion = "1.0");