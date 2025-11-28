namespace Atc.Azure.IoT.CLI.Factories;

public static class IotHubServiceFactory
{
    public static IoTHubService Create(
        string connectionString,
        ILoggerFactory? loggerFactory = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var iotHubOptions = new IotHubOptions { ConnectionString = connectionString };

        return Create(iotHubOptions, loggerFactory);
    }

    public static IoTHubService Create(
        IotHubOptions options,
        ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        var iotHubModuleService = new IoTHubModuleService(
            options,
            loggerFactory);

        return new IoTHubService(
            iotHubModuleService,
            options,
            loggerFactory);
    }
}