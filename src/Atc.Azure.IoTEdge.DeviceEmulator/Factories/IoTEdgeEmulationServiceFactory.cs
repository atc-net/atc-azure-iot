namespace Atc.Azure.IoTEdge.DeviceEmulator.Factories;

public static class IoTEdgeEmulationServiceFactory
{
    public static IoTEdgeEmulationService BuildIoTEdgeEmulationService(
        ILoggerFactory loggerFactory,
        IDockerService dockerService,
        IIoTHubService iotHubService)
        => new(
            loggerFactory,
            new FileService(),
            dockerService,
            iotHubService,
            new AzureIoTHubService(
                loggerFactory,
                iotHubService));
}