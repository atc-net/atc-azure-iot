// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

public static partial class IoTEdgeDeviceBuilderExtensions
{
    [LoggerMessage(
        EventId = 200,
        Level = LogLevel.Information,
        Message = "Loaded deployment manifest '{Path}' with {RouteCount} routes and desired properties for {ModuleCount} modules.")]
    private static partial void LogDeploymentManifestLoaded(
        this ILogger logger,
        string path,
        int routeCount,
        int moduleCount);

    [LoggerMessage(
        EventId = 201,
        Level = LogLevel.Information,
        Message = "Provisioning IoT Edge device '{DeviceId}' in IoT Hub '{IotHubHostName}'.")]
    private static partial void LogProvisioningDevice(
        this ILogger logger,
        string deviceId,
        string iotHubHostName);

    [LoggerMessage(
        EventId = 202,
        Level = LogLevel.Information,
        Message = "IoT Edge device '{DeviceId}' is ready with modules {Modules}. Modules connect: {Mode}.")]
    private static partial void LogDeviceReady(
        this ILogger logger,
        string deviceId,
        string modules,
        string mode);

    [LoggerMessage(
        EventId = 203,
        Level = LogLevel.Error,
        Message = "Provisioning IoT Edge device '{DeviceId}' failed: {ErrorMessage}")]
    private static partial void LogProvisioningFailed(
        this ILogger logger,
        Exception exception,
        string deviceId,
        string errorMessage);
}