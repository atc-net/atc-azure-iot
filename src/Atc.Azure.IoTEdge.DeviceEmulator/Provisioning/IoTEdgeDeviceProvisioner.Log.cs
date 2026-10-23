namespace Atc.Azure.IoTEdge.DeviceEmulator.Provisioning;

/// <summary>
/// IoTEdgeDeviceProvisioner LoggerMessages.
/// </summary>
public sealed partial class IoTEdgeDeviceProvisioner
{
    private readonly ILogger<IoTEdgeDeviceProvisioner> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceCreated,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Created IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    private partial void LogDeviceCreated(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ModuleCreated,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Created module identity '{ModuleId}' on IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    private partial void LogModuleCreated(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ModuleKeysAssigned,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Assigned symmetric keys to module identity '{ModuleId}' on IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    private partial void LogModuleKeysAssigned(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ModuleDesiredPropertiesApplied,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Applied desired properties from the deployment manifest to module '{ModuleId}' on IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    private partial void LogModuleDesiredPropertiesApplied(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ModuleDesiredPropertiesNotApplied,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Could not apply desired properties from the deployment manifest to module '{ModuleId}' on IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    private partial void LogModuleDesiredPropertiesNotApplied(
        string iotHubHostName,
        string deviceId,
        string moduleId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioned,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Provisioned IoT Edge device '{DeviceId}' with {ModuleCount} module identities in IoT Hub '{IotHubHostName}'.")]
    private partial void LogDeviceProvisioned(
        string iotHubHostName,
        string deviceId,
        int moduleCount,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceDeprovisioned,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Deleted IoT Edge device '{DeviceId}' from IoT Hub '{IotHubHostName}'.")]
    private partial void LogDeviceDeprovisioned(
        string iotHubHostName,
        string deviceId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}