namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// IoTHubModuleService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class IoTHubModuleService
{
    private readonly ILogger<IoTHubModuleService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubModuleService.MethodCallFailedDeviceNotFound,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Calling method '{methodName}' with jsonPayload '{jsonPayload}' failed - device not found by deviceId '{deviceId}': {errorMessage}")]
    private partial void LogMethodCallFailedDeviceNotFound(
        string deviceId,
        string methodName,
        [StringSyntax(StringSyntaxAttribute.Json)]
        string jsonPayload,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubModuleService.MethodCallFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Calling method '{methodName}' on device with id {deviceId} with jsonPayload '{jsonPayload}' failed: {errorMessage}")]
    private partial void LogMethodCallFailed(
        string deviceId,
        string methodName,
        [StringSyntax(StringSyntaxAttribute.Json)]
        string jsonPayload,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}