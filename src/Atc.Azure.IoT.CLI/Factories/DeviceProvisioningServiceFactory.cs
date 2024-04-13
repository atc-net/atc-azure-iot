namespace Atc.Azure.IoT.CLI.Factories;

public static class DeviceProvisioningServiceFactory
{
    public static DeviceProvisioningService Create(
        ILoggerFactory loggerFactory,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var dpsOptions = new DeviceProvisioningServiceOptions { ConnectionString = connectionString };

        return Create(loggerFactory, dpsOptions);
    }

    public static DeviceProvisioningService Create(
        ILoggerFactory loggerFactory,
        DeviceProvisioningServiceOptions options)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return new DeviceProvisioningService(
            loggerFactory,
            options);
    }
}