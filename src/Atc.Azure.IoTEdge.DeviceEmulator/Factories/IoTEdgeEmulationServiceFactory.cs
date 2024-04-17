using AzureIoTHubService = Atc.Azure.IoTEdge.DeviceEmulator.Services.AzureIoTHubService;
using IoTEdgeEmulationService = Atc.Azure.IoTEdge.DeviceEmulator.Services.IoTEdgeEmulationService;

namespace Atc.Azure.IoTEdge.DeviceEmulator.Factories;

public static class IoTEdgeEmulationServiceFactory
{
    public static IoTEdgeEmulationService BuildIoTEdgeEmulationService(
        ILogger logger,
        IDockerService dockerService,
        IRegistryManagerWrapper registryManagerWrapper)
        => new(
            logger,
            new FileService(),
            dockerService,
            new AzureIoTHubService(logger, registryManagerWrapper));
}