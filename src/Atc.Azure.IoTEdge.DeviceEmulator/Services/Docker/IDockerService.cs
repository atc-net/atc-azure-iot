namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Docker;

public interface IDockerService
{
    Task<bool> StartContainer(
        string containerName,
        string deviceConnectionString,
        CancellationToken cancellationToken);

    Task<bool> StopContainer(
        CancellationToken cancellationToken);

    Task<bool> EnsureIotEdgeEmulatorIsReady(
        string moduleName,
        int numberOfRetries = 15,
        int secondsBetweenRetries = 3);

    Task<(bool Succeeded, IDictionary<string, string>? IotEdgeVariables)> GetIotEdgeVariablesFromContainer(
        string moduleName);
}