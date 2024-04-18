namespace SimulationModule;

/// <summary>
/// SimulationModuleService LoggerMessages.
/// </summary>
public sealed partial class SimulationModuleService
{
    private readonly ILogger logger;

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStarted,
        Level = LogLevel.Trace,
        Message = "Successfully started module '{moduleName}'.")]
    private partial void LogModuleStarted(
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStopping,
        Level = LogLevel.Trace,
        Message = "Stopping module '{moduleName}'.")]
    private partial void LogModuleStopping(
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStopped,
        Level = LogLevel.Trace,
        Message = "Successfully stopped module '{moduleName}'.")]
    private partial void LogModuleStopped(
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleClientStarted,
        Level = LogLevel.Trace,
        Message = "Successfully started moduleClient for module '{moduleName}'.")]
    private partial void LogModuleClientStarted(
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleClientStopped,
        Level = LogLevel.Trace,
        Message = "Successfully stopped moduleClient for module '{moduleName}'.")]
    private partial void LogModuleClientStopped(
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ConnectionStatusChange,
        Level = LogLevel.Trace,
        Message = "Connection changed - status: '{status}', reason: '{reason}'.")]
    private partial void LogConnectionStatusChange(
        ConnectionStatus status,
        ConnectionStatusChangeReason reason);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodCalled,
        Level = LogLevel.Trace,
        Message = "Method '{methodName}' was called on module '{moduleName}'.")]
    private partial void LogMethodCalled(
        string methodName,
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodCallCompleted,
        Level = LogLevel.Trace,
        Message = "Method '{methodName}' completed on module '{moduleName}'.")]
    private partial void LogMethodCallCompleted(
        string methodName,
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestEmpty,
        Level = LogLevel.Error,
        Message = "No data in request when method '{methodName}' was called on module '{moduleName}'.")]
    private partial void LogMethodRequestEmpty(
        string methodName,
        string moduleName);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestDeserializationError,
        Level = LogLevel.Error,
        Message = "Could not deserialize request when method '{methodName}' was called on module '{moduleName}': '{errorMessage}'.")]
    private partial void LogMethodRequestDeserializationError(
        string methodName,
        string moduleName,
        string errorMessage);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestUriParsingError,
        Level = LogLevel.Error,
        Message = "Could not parse url '{url}' from request when method '{methodName}' was called on module '{moduleName}': '{errorMessage}'.")]
    private partial void LogMethodRequestUriParsingError(
        string url,
        string methodName,
        string moduleName,
        string errorMessage);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestError,
        Level = LogLevel.Error,
        Message = "Could not process request when method '{methodName}' was called on module '{moduleName}': '{errorMessage}'.")]
    private partial void LogMethodRequestError(
        string methodName,
        string moduleName,
        string errorMessage);
}