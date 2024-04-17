namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;

/// <summary>
/// SystemEnvironmentService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public partial class SystemEnvironmentService
{
    private readonly ILogger logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.EnvironmentInjectedVariable,
        Level = LogLevel.Trace,
        Message = "Injected variable: {key}={value}.")]
    private partial void LogEnvironmentInjectedVariable(string key, string value);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.EnvironmentPlatformNotImplemented,
        Level = LogLevel.Error,
        Message = "{message}")]
    private partial void LogEnvironmentPlatformNotImplemented(string message);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.EnvironmentPlatformNotSupported,
        Level = LogLevel.Error,
        Message = "{message}")]
    private partial void LogEnvironmentPlatformNotSupported(string message);
}