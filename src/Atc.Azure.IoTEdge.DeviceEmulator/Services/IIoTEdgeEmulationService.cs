namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

public interface IIoTEdgeEmulationService
{
    Task<bool> StartEmulator(
        string filePath,
        string iotHubConnectionString,
        string deviceId,
        string containerName,
        CancellationToken cancellationToken);

    Task<bool> StopEmulator(
        CancellationToken cancellationToken);

    void EnsureProperIotEdgeEnvironmentVariables(
        IDictionary<string, string> environmentVariables);
}