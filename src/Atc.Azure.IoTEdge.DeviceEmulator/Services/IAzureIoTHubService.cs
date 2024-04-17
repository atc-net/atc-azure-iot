namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

public interface IAzureIoTHubService
{
    (bool Succeeded, string EmulationManifest) TransformTemplateToEmulationManifest(
            string templateContent);

    Task<(bool Succeeded, string DeviceConnectionString)> ProvisionIotEdgeDevice(
        string emulationManifest,
        string iotHubConnectionString,
        string deviceId,
        CancellationToken cancellationToken);
}