namespace Atc.Azure.IoT.CLI.Factories;

public static class IotHubServiceFactory
{
    public static IoTHubService Create(
        ILoggerFactory loggerFactory,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var iothubOptions = new IotHubOptions { ConnectionString = connectionString };

        return Create(loggerFactory, iothubOptions);
    }

    public static IoTHubService Create(
        ILoggerFactory loggerFactory,
        IotHubOptions options)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        var iotHubModuleService = new IoTHubModuleService(
            loggerFactory,
            options);

        return new IoTHubService(
            loggerFactory,
            iotHubModuleService,
            options);
    }
}