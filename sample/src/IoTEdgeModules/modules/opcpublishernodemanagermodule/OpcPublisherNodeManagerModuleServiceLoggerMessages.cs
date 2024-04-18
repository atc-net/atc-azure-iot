namespace OpcPublisherNodeManagerModule;

/// <summary>
/// OpcPublisherNodeManagerModuleService LoggerMessages.
/// </summary>
public sealed partial class OpcPublisherNodeManagerModuleService
{
    private readonly ILogger<OpcPublisherNodeManagerModuleService> logger;

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStarted,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully started module '{moduleName}'.")]
    private partial void LogModuleStarted(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStopping,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Stopping module '{moduleName}'.")]
    private partial void LogModuleStopping(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleStopped,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully stopped module '{moduleName}'.")]
    private partial void LogModuleStopped(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleClientStarted,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully started moduleClient for module '{moduleName}'.")]
    private partial void LogModuleClientStarted(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ModuleClientStopped,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully stopped moduleClient for module '{moduleName}'.")]
    private partial void LogModuleClientStopped(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.ConnectionStatusChange,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Connection changed - status: '{status}', reason: '{reason}'.")]
    private partial void LogConnectionStatusChange(
        ConnectionStatus status,
        ConnectionStatusChangeReason reason,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodCalled,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Method '{methodName}' was called on module '{moduleName}'.")]
    private partial void LogMethodCalled(
        string methodName,
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodCallCompleted,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Method '{methodName}' completed on module '{moduleName}'.")]
    private partial void LogMethodCallCompleted(
        string methodName,
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestEmpty,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - No data in request when method '{methodName}' was called on module '{moduleName}'.")]
    private partial void LogMethodRequestEmpty(
        string methodName,
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestDeserializationError,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Could not deserialize request when method '{methodName}' was called on module '{moduleName}': '{errorMessage}'.")]
    private partial void LogMethodRequestDeserializationError(
        string methodName,
        string moduleName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestUriParsingError,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Could not parse url '{url}' from request when method '{methodName}' was called on module '{moduleName}': '{errorMessage}'.")]
    private partial void LogMethodRequestUriParsingError(
        string url,
        string methodName,
        string moduleName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestUriFormatExceptionError,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Exception while parsing endpointUrl '{endpointUrl}': '{errorMessage}'")]
    private partial void LogMethodRequestUriFormatExceptionError(
        string endpointUrl,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.MethodRequestError,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Could not process request when method '{methodName}' was called on module '{moduleName}': '{errorMessage}'.")]
    private partial void LogMethodRequestError(
        string methodName,
        string moduleName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ConfigurationFileMissing,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - The node configuration file '{publisherNodeConfigurationFilename}' does not exist.")]
    private partial void LogConfigurationFileNotFound(
        string publisherNodeConfigurationFilename,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ConfigurationFileLoadError,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Loading of the node configuration file failed. Check the file exist and have correct syntax: '{errorMessage}'.")]
    private partial void LogConfigurationFileLoadError(
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ConfigurationFileWriteError,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Writing to the node configuration file failed. Check the file exist and have correct syntax: '{errorMessage}'.")]
    private partial void LogConfigurationFileWriteError(
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}