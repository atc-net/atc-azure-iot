namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

/// <summary>
/// AzureIoTHubService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public partial class AzureIoTHubService
{
    private readonly ILogger logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubProvisionIotEdgeDeviceSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully provisioned IoT Edge Device '{deviceId}'.")]
    private partial void LogIotHubProvisionIotEdgeDeviceSucceeded(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubProvisionIotEdgeDeviceFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to provision IoT Edge Device '{deviceId}': '{errorMessage}'.")]
    private partial void LogIotHubProvisionIotEdgeDeviceFailed(
        string deviceId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubProvisionIotEdgeDeviceMissingHostName,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - {message}.")] // TODO: Improve
    private partial void LogIotHubProvisionIotEdgeDeviceMissingHostName(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubProvisionIotEdgeDeviceInvalidManifest,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - {message}.")] // TODO: Improve
    private partial void LogIotHubProvisionIotEdgeDeviceInvalidManifest(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubTransformTemplateToManifestSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully transformed template to manifest.")]
    private partial void LogIotHubTransformTemplateToManifestSucceeded(
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubTransformTemplateToManifestFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to transform template to manifest: '{errorMessage}'.")]
    private partial void LogIotHubTransformTemplateToManifestFailed(
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubAddModulesToIotEdgeDeviceSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully added new modules to IoT Edge Device '{deviceId}'.")]
    private partial void LogIotHubAddModulesToIotEdgeDeviceSucceeded(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubAddModulesToIotEdgeDeviceFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to add new modules to IoT Edge Device '{deviceId}': '{errorMessage}'.")]
    private partial void LogIotHubAddModulesToIotEdgeDeviceFailed(
        string deviceId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubApplyConfigurationContentToIotEdgeDeviceSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully added configuration content to IoT Edge Device '{deviceId}'.")]
    private partial void LogIotHubApplyConfigurationContentToIotEdgeDeviceSucceeded(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubApplyConfigurationContentToIotEdgeDeviceFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to add configuration content to IoT Edge Device '{deviceId}': '{errorMessage}'.")]
    private partial void LogIotHubApplyConfigurationContentToIotEdgeDeviceFailed(
        string deviceId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubRemoveModulesFromIotEdgeDeviceSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully removed old modules from IoT Edge Device '{deviceId}'.")]
    private partial void LogIotHubRemoveModulesFromIotEdgeDeviceSucceeded(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubRemoveModulesFromIotEdgeDeviceFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to remove old modules from IoT Edge Device '{deviceId}': '{errorMessage}'.")]
    private partial void LogIotHubRemoveModulesFromIotEdgeDeviceFailed(
        string deviceId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubRemoveIotEdgeDeviceSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully removed IoT Edge Device '{deviceId}'.")]
    private partial void LogIotHubRemoveIotEdgeDeviceSucceeded(
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotHubRemoveIotEdgeDeviceFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to remove IoT Edge Device '{deviceId}': '{errorMessage}'.")]
    private partial void LogIotHubRemoveIotEdgeDeviceFailed(
        string deviceId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}