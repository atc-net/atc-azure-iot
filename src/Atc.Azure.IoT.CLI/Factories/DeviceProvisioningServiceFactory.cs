namespace Atc.Azure.IoT.CLI.Factories;

public static class DeviceProvisioningServiceFactory
{
    public static DeviceProvisioningService Create(
        string connectionString,
        ILoggerFactory? loggerFactory = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var dpsOptions = new DeviceProvisioningServiceOptions { ConnectionString = connectionString };

        return Create(dpsOptions, loggerFactory);
    }

    public static DeviceProvisioningService Create(
        DeviceProvisioningServiceOptions options,
        ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new DeviceProvisioningService(
            options,
            loggerFactory);
    }
}