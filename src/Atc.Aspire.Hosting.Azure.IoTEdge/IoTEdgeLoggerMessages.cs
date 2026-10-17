namespace Atc.Aspire.Hosting.Azure.IoTEdge;

/// <summary>
/// LoggerMessages for the IoT Edge resources.
/// </summary>
internal static partial class IoTEdgeLoggerMessages
{
    [LoggerMessage(
        EventId = 200,
        Level = LogLevel.Information,
        Message = "Loaded deployment manifest '{Path}' with {RouteCount} routes and desired properties for {ModuleCount} modules.")]
    internal static partial void LogDeploymentManifestLoaded(
        this ILogger logger,
        string path,
        int routeCount,
        int moduleCount);

    [LoggerMessage(
        EventId = 201,
        Level = LogLevel.Information,
        Message = "Provisioning IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    internal static partial void LogProvisioningDevice(
        this ILogger logger,
        string deviceId,
        string iotHubHostName);

    [LoggerMessage(
        EventId = 202,
        Level = LogLevel.Information,
        Message = "IoT Edge device '{DeviceId}' is ready with modules {Modules}. Modules connect: {Mode}.")]
    internal static partial void LogDeviceReady(
        this ILogger logger,
        string deviceId,
        string modules,
        string mode);

    [LoggerMessage(
        EventId = 203,
        Level = LogLevel.Error,
        Message = "Provisioning IoT Edge device '{DeviceId}' failed: {ErrorMessage}")]
    internal static partial void LogProvisioningFailed(
        this ILogger logger,
        Exception exception,
        string deviceId,
        string errorMessage);
}