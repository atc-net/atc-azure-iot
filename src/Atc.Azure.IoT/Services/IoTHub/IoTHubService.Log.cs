namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// IoTHubService LoggerMessages.
/// </summary>
public sealed partial class IoTHubService
{
    private readonly ILogger<IoTHubService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingRegistryStatistics,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieving registry statistics on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingRegistryStatistics(
        string iotHubHostName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDevice,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve iot edge device for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingDevice(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not find iot edge device by deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogIotEdgeDeviceNotFound(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDeviceSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved iot edge device for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveIotDeviceSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingDeviceConnectionString,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve iot edge device connection-string for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingDeviceConnectionString(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotEdgeDeviceConnectionStringSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved iot edge device connection-string for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveIotEdgeDeviceConnectionStringSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingModuleConnectionString,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve module connection-string for deviceId '{DeviceId}' and moduleId '{ModuleId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingModuleConnectionString(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotEdgeDeviceModuleNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not find module '{ModuleId}' on iot device '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogIotEdgeDeviceModuleNotFound(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveModuleConnectionStringSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved module connection-string for deviceId '{DeviceId}' and moduleId '{ModuleId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveModuleConnectionStringSucceeded(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDeviceTwins,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve iot device twins on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingIotDeviceTwins(
        string iotHubHostName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDeviceTwinsSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved '{deviceCount}' iot device twins from iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveIotDeviceTwinsSucceeded(
        string iotHubHostName,
        int deviceCount,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotDeviceTwin,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve iot device twin for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingDeviceTwin(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceTwinNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not find iot device twin by deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogIotDeviceTwinNotFound(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotDeviceTwinSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved iot device twin for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveIotDeviceTwinSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdatingIotDeviceTwin,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to update device twin for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogUpdatingDeviceTwin(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdateIotDeviceTwinSucceeded,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Updated iot device twin for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogUpdateIotDeviceTwinSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdatingModuleTwinDesiredProperties,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to update module twin desired properties for deviceId '{DeviceId}' and moduleId '{ModuleId}' on iot hub '{IotHubHostName}'")]
    private partial void LogUpdatingDeviceTwinDesiredProperties(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.UpdateModuleTwinDesiredPropertiesSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Updated module twin desired properties for deviceId '{DeviceId}' and moduleId '{ModuleId}' on iot hub '{IotHubHostName}'")]
    private partial void LogUpdateModuleTwinDesiredPropertiesSucceeded(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotEdgeDeviceModules,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve iot edge device modules for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingIotEdgeDeviceModules(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotEdgeDeviceModulesNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not find iot edge device modules for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogIotEdgeDeviceModulesNotFound(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotEdgeDeviceModulesSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved iot edge device modules for deviceId '{DeviceId}' from iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveIotEdgeDeviceModulesSucceeded(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrievingIotEdgeDeviceTwinModule,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting to retrieve iot edge device twin modules for deviceId '{DeviceId}' and moduleId '{ModuleId}' on iot hub '{IotHubHostName}'")]
    private partial void LogRetrievingIotEdgeDeviceTwinModule(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotEdgeDeviceTwinModuleNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not find iot edge device twin module for deviceId '{DeviceId}' and moduleId '{ModuleId}' on iot hub '{IotHubHostName}'")]
    private partial void LogIotEdgeDeviceTwinModuleNotFound(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.RetrieveIotEdgeDeviceTwinModuleSucceeded,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Retrieved iot edge device twin modules for deviceId '{DeviceId}' and moduleId '{ModuleId}' from iot hub '{IotHubHostName}'")]
    private partial void LogRetrieveIotEdgeDeviceTwinModuleSucceeded(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceModuleRemoved,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully removed module '{ModuleId}' from iot device '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogIotDeviceModuleRemoved(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceModuleNotFoundOnRemove,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Module '{ModuleId}' was not removed from iot device '{DeviceId}' on iot hub '{IotHubHostName}', because it doesn't exist")]
    private partial void LogIotDeviceModuleNotFoundOnRemove(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.AddedModulesToIotEdgeDevice,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully added new modules to deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogAddedModulesToIotEdgeDevice(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.AppliedConfigurationContent,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully added configuration content for deviceId '{DeviceId}' on iot hub '{IotHubHostName}'")]
    private partial void LogAppliedConfigurationContent(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceDeleted,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully deleted iot device '{DeviceId}' from iot hub '{IotHubHostName}'")]
    private partial void LogIotDeviceDeleted(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IoTHubService.IotDeviceNotFoundOnDelete,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Iot device '{DeviceId}' was not deleted from iot hub '{IotHubHostName}', because it doesn't exist")]
    private partial void LogIotDeviceNotFoundOnDelete(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}