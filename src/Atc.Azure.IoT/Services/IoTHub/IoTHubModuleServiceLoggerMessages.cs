namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// IoTHubModuleService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class IoTHubModuleService
{
    private readonly ILogger<IoTHubModuleService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubModuleService.MethodCallFailed,
        Level = LogLevel.Error,
        Message = "Failed to call method '{MethodName}' on device with ID {DeviceId}, with payload '{JsonPayload}'")]
    private partial void LogMethodCallFailed(
        IotHubException exception,
        string deviceId,
        string methodName,
        [StringSyntax(StringSyntaxAttribute.Json)]
        string jsonPayload);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubModuleService.MethodCallTransientError,
        Level = LogLevel.Warning,
        Message = "Transient error {ErrorCode} occurred while calling method '{MethodName}' on device with ID {DeviceId}. " +
                  "This was attempt {RetryCount} of {TotalRetryCount}. " +
                  "Waiting {WaitTimeInSeconds} seconds before retrying")]
    private partial void LogMethodCallTransientError(
        ErrorCode errorCode,
        string methodName,
        string deviceId,
        int retryCount,
        int totalRetryCount,
        double waitTimeInSeconds);
}