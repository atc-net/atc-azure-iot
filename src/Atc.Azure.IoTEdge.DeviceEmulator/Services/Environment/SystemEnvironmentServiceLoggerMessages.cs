namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;

/// <summary>
/// SystemEnvironmentService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public partial class SystemEnvironmentService
{
    private readonly ILogger<SystemEnvironmentService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.EnvironmentInjectedVariable,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Injected variable: {key}={value}.")]
    private partial void LogEnvironmentInjectedVariable(
        string key,
        string value,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.EnvironmentPlatformNotSupported,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - {message}")]
    private partial void LogEnvironmentPlatformNotSupported(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}