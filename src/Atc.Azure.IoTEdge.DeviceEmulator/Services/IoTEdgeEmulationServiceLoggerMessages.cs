namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

/// <summary>
/// IoTEdgeEmulationService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public partial class IoTEdgeEmulationService
{
    private readonly ILogger logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorStarting,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - IoT Edge Emulator is starting for device '{deviceId}'.")]
    private partial void LogIotEdgeEmulatorStarting(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorGetTemplateContentSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully retrieved template content.")]
    private partial void LogIotEdgeEmulatorGetTemplateContentSucceeded(
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorGetTemplateContentFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to retrieve template content: '{errorMessage}'")]
    private partial void LogIotEdgeEmulatorGetTemplateContentFailed(
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorEnsuringProperVariables,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Ensuring proper IoT Edge variables are set locally for emulator.")]
    private partial void LogIotEdgeEmulatorEnsuringProperVariables(
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorStopping,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - IoT Edge Emulator is stopping for device '{deviceId}'")]
    private partial void LogIotEdgeEmulatorStopping(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}