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
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully started module '{ModuleName}'.")]
    private partial void LogModuleStarted(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStopping,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Stopping module '{ModuleName}'.")]
    private partial void LogModuleStopping(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStopped,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully stopped module '{ModuleName}'.")]
    private partial void LogModuleStopped(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleClientStarted,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully started moduleClient for module '{ModuleName}'.")]
    private partial void LogModuleClientStarted(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleClientStopped,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully stopped moduleClient for module '{ModuleName}'.")]
    private partial void LogModuleClientStopped(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ConnectionStatusChange,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Connection changed - status: '{Status}', reason: '{Reason}'.")]
    private partial void LogConnectionStatusChange(
        ConnectionStatus status,
        ConnectionStatusChangeReason reason,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodCalled,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Method '{MethodName}' was called on module '{ModuleName}'.")]
    private partial void LogMethodCalled(
        string methodName,
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodCallCompleted,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Method '{MethodName}' completed on module '{ModuleName}'.")]
    private partial void LogMethodCallCompleted(
        string methodName,
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestEmpty,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - No data in request when method '{MethodName}' was called on module '{ModuleName}'.")]
    private partial void LogMethodRequestEmpty(
        string methodName,
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestDeserializationError,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not deserialize request when method '{MethodName}' was called on module '{ModuleName}': '{ErrorMessage}'.")]
    private partial void LogMethodRequestDeserializationError(
        string methodName,
        string moduleName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestUriParsingError,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not parse url '{url}' from request when method '{MethodName}' was called on module '{ModuleName}': '{ErrorMessage}'.")]
    private partial void LogMethodRequestUriParsingError(
        string url,
        string methodName,
        string moduleName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestError,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not process request when method '{MethodName}' was called on module '{ModuleName}': '{ErrorMessage}'.")]
    private partial void LogMethodRequestError(
        string methodName,
        string moduleName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}