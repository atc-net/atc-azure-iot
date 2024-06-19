namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// IoTHubService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class IoTHubService
{
    private readonly ILogger<IoTHubService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.Failure,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - An unexpected exception of type '{exceptionType}' occurred when executing call against iot hub '{iotHubHostName}': {errorMessage}")]
    private partial void LogFailure(
        string iotHubHostName,
        string exceptionType,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingRegistryStatistics,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieving registry statistics on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingRegistryStatistics(
        string iotHubHostName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDevice,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot device for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingDevice(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceNotFound,
        Level = LogLevel.Warning,
        Message = "{callerMethodName}({callerLineNumber}) - Could not find iot device by deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogIotEdgeDeviceNotFound(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDeviceSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved iot device for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveIotDeviceSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDevices,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot devices on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingDevices(
        string iotHubHostName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDevicesSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved {count} iot devices from on iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveIotDevicesSucceeded(
        string iotHubHostName,
        int count,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingDeviceConnectionString,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot device connection-string for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingDeviceConnectionString(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveDeviceConnectionStringSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved iot device connection-string for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveDeviceConnectionStringSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDeviceTwins,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot device twins on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingIotDeviceTwins(
        string iotHubHostName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDeviceTwinsSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved '{deviceCount}' iot device twins from iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveIotDeviceTwinsSucceeded(
        string iotHubHostName,
        int deviceCount,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDeviceTwin,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot device twin for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingDeviceTwin(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceTwinNotFound,
        Level = LogLevel.Warning,
        Message = "{callerMethodName}({callerLineNumber}) - Could not find iot device twin by deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogIotDeviceTwinNotFound(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDeviceTwinSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved iot device twin for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveIotDeviceTwinSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdatingIotDeviceTwin,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to update device twin for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogUpdatingDeviceTwin(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceTwinNotUpdated,
        Level = LogLevel.Warning,
        Message = "{callerMethodName}({callerLineNumber}) - Could not update iot device twin by deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogIotDeviceTwinNotUpdated(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdateIotDeviceTwinSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Updated iot device twin for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogUpdateIotDeviceTwinSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdatingModuleTwinDesiredProperties,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to update module twin desired properties for deviceId '{deviceId}' and moduleId '{moduleId}' on iot hub '{iotHubHostName}'")]
    private partial void LogUpdatingDeviceTwinDesiredProperties(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdateModuleTwinDesiredPropertiesSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Updated module twin desired properties for deviceId '{deviceId}' and moduleId '{moduleId}' on iot hub '{iotHubHostName}'")]
    private partial void LogUpdateModuleTwinDesiredPropertiesSucceeded(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.ModuleTwinDesiredPropertiesNotUpdated,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Could not update module twin desired properties for deviceId '{deviceId}' and moduleId '{moduleId}' on iot hub '{iotHubHostName}'")]
    private partial void LogModuleTwinDesiredPropertiesNotUpdated(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotEdgeDeviceModules,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot edge device modules for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingIotEdgeDeviceModules(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotEdgeDeviceModulesNotFound,
        Level = LogLevel.Warning,
        Message = "{callerMethodName}({callerLineNumber}) - Could not find iot edge device modules for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogIotEdgeDeviceModulesNotFound(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotEdgeDeviceModulesSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved iot edge device modules for deviceId '{deviceId}' from iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveIotEdgeDeviceModulesSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotEdgeDeviceTwinModule,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Starting to retrieve iot edge device twin modules for deviceId '{deviceId}' and moduleId '{moduleId}' on iot hub '{iotHubHostName}'")]
    private partial void LogRetrievingIotEdgeDeviceTwinModule(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotEdgeDeviceTwinModuleNotFound,
        Level = LogLevel.Warning,
        Message = "{callerMethodName}({callerLineNumber}) - Could not find iot edge device twin module for deviceId '{deviceId}' and moduleId '{moduleId}' on iot hub '{iotHubHostName}'")]
    private partial void LogIotEdgeDeviceTwinModuleNotFound(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotEdgeDeviceTwinModuleSucceeded,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Retrieved iot edge device twin modules for deviceId '{deviceId}' and moduleId '{moduleId}' from iot hub '{iotHubHostName}'")]
    private partial void LogRetrieveIotEdgeDeviceTwinModuleSucceeded(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceModuleRemoved,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully removed module '{moduleId}' from iot device '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogIotDeviceModuleRemoved(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.AddedModulesToIotEdgeDevice,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully added new modules to deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogAddedModulesToIotEdgeDevice(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.AppliedConfigurationContent,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully added configuration content for deviceId '{deviceId}' on iot hub '{iotHubHostName}'")]
    private partial void LogAppliedConfigurationContent(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceDeleted,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully deleted iot device '{deviceId}' from iot hub '{iotHubHostName}'")]
    private partial void LogIotDeviceDeleted(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}