namespace Atc.Azure.IoTEdge.DeviceEmulator.Factories;

public static class IoTEdgeEmulationServiceFactory
{
    public static IoTEdgeEmulationService BuildIoTEdgeEmulationService(
        IDockerService dockerService,
        IIoTHubService iotHubService,
        ILoggerFactory? loggerFactory = null)
        => new(
            new FileService(),
            dockerService,
            iotHubService,
            new AzureIoTHubService(
                iotHubService,
                loggerFactory),
            loggerFactory);
}