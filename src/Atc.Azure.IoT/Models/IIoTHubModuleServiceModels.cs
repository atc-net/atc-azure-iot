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

/// <summary>
/// Retrieve module logs and upload to Azure Blob Storage.
/// </summary>
/// <param name="SasUrl">Shared Access Signature URL with write access to Azure Blob Storage container.</param>
/// <param name="Items">An array with id and filter tuples.</param>
/// <param name="Encoding">Either gzip or none. Default is none.</param>
/// <param name="ContentType">Either json or text. Default is text.</param>
public record UploadModuleLogsRequest(
    [property: JsonPropertyName("sasUrl")] Uri SasUrl,
    [property: JsonPropertyName("items")] IEnumerable<LogRequestItem> Items,
    [property: JsonPropertyName("encoding")] LogContentEncoding Encoding = LogContentEncoding.None,
    [property: JsonPropertyName("contentType")] LogContentType ContentType = LogContentType.Text)
    : SchemaVersion10;

/// <param name="Id">
/// A regular expression that specifies the module name.
/// It can match multiple modules on an edge device.
/// .NET Regular Expressions format is expected.
/// If multiple items have an ID that matches the same module,
/// only the filter options of the first matching ID apply to that module.</param>
/// <param name="Filter">
/// Log filters to apply to the modules matching the id regular expression in the tuple.
/// </param>
public record LogRequestItem(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("filter")] ModuleLogFilter? Filter = null);

/// <param name="Tail">
/// Number of log lines in the past to get, starting from the latest.
/// </param>
/// <param name="Since">
/// Only return logs since this time, as a rfc3339 timestamp, UNIX timestamp, or a duration (days (d), hours (h), minutes (m)).
/// For example, a duration for one day, 12 hours, and 30 minutes can be specified as 1 day 12 hours 30 minutes or 1d 12h 30m.
/// If both <see cref="Tail"/> and <see cref="Since"/> are specified,
/// the logs are filtered using the <see cref="Since"/> value first,
/// then the <see cref="Tail"/> value is applied to the result.
/// </param>
/// <param name="Until">
/// Only return logs before the specified time, as an rfc3339 timestamp, UNIX timestamp, or duration (days (d), hours (h), minutes (m)).
/// For example, a duration of 90 minutes can be specified as 90 minutes or 90m.
/// If both <see cref="Tail"/> and <see cref="Since"/> are specified,
/// the logs are filtered using the since value first, then the tail value is applied to the result.
/// </param>
/// <param name="LogLevel">
/// Filter log lines equal to the specified log level.
/// Log lines should follow the recommended logging format and use the
/// <see href="https://en.wikipedia.org/wiki/Syslog#Severity_level">Syslog severity level standard</see>.
/// If you need to filter by multiple log level severity values, use regex matching,
/// provided the module uses a consistent format for different severity levels.
/// </param>
/// <param name="Regex">
/// Filter log lines that match the specified regular expression using
/// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions">.NET Regular Expressions format.</see>
/// </param>
public record ModuleLogFilter(
    [property: JsonPropertyName("tail")] int? Tail = null,
    [property: JsonPropertyName("since")] string? Since = null,
    [property: JsonPropertyName("until")] string? Until = null,
    [property: JsonPropertyName("loglevel")] int? LogLevel = null,
    [property: JsonPropertyName("message")] string? Regex = null);

public record GetTaskStatusRequest(
    [property: JsonPropertyName("correlationId")] string CorrelationId)
    : SchemaVersion10;

public record Response<T>(int StatusCode, T Payload);

public record LogResponse(
    [property: JsonPropertyName("status")] BackgroundTaskRunStatus Status,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("correlationId")] string CorrelationId,
    [property: JsonPropertyName("errorCode")] string? ErrorCode);

public enum BackgroundTaskRunStatus
{
    Unknown,
    NotStarted,
    Running,
    Completed,
    Cancelled,
    Failed,
}

public enum LogContentEncoding
{
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = "gzip")]
    Gzip,
}

public enum LogContentType
{
    [EnumMember(Value = "text")]
    Text,

    [EnumMember(Value = "json")]
    Json,
}

public abstract record SchemaVersion10
{
    [property: JsonPropertyName("schemaVersion")]
    public string SchemaVersion => "1.0";
}