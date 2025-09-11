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
    [property: JsonPropertyName("id")] string Id)
    : SchemaVersion10;

/// <summary>
/// Retrieve module logs with a support bundle and upload a zip file to Azure Blob Storage
/// </summary>
/// <param name="SasUrl">Shared Access Signature URL with write access to Azure Blob Storage container</param>
/// <param name="Since">
/// Return logs since this time, as an RFC 3339 timestamp, UNIX timestamp, or a duration (days (d), hours (h), minutes (m)).
/// For example, specify one day, 12 hours, and 30 minutes as 1 day 12 hours 30 minutes or 1d 12h 30m.</param>
/// <param name="Until">
/// Return logs before this time, as an RFC 3339 timestamp, UNIX timestamp, or duration (days (d), hours (h), minutes (m)).
/// For example, specify 90 minutes as 90 minutes or 90m
/// </param>
/// <param name="EdgeRuntimeOnly">If <see langword="true" />, return logs only from Edge Agent, Edge Hub, and the Edge Security Daemon
/// Default: <see langword="false" />.</param>
public record UploadSupportBundleRequest(
    [property: JsonPropertyName("sasUrl")] Uri SasUrl,
    [property: JsonPropertyName("since")] string? Since = null,
    [property: JsonPropertyName("until")] string? Until = null,
    [property: JsonPropertyName("edgeRuntimeOnly")] bool EdgeRuntimeOnly = false)
    : SchemaVersion10;

public record GetTaskStatusRequest(
    [property: JsonPropertyName("correlationId")] string CorrelationId)
    : SchemaVersion10;

public record Response<T>(int StatusCode, T Payload);

public record LogResponse(
    [property: JsonPropertyName("status")] BackgroundTaskRunStatus Status,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("correlationId")] string CorrelationId);

public enum BackgroundTaskRunStatus
{
    Unknown,
    NotStarted,
    Running,
    Completed,
    Cancelled,
    Failed,
}

public abstract record SchemaVersion10
{
    [property: JsonPropertyName("schemaVersion")]
    public string SchemaVersion => "1.0";
}