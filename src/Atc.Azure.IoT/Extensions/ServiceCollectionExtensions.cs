// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Static class holding extension methods for service collection wire-up for IoT related Services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures IoTHub related services and registers them to the provided IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="iotHubOptions">The iotHubOptions.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="IotHubOptions"/> service is not registered or the connection string is missing.</exception>
    public static IServiceCollection ConfigureIotHubServices(
        this IServiceCollection services,
        IotHubOptions iotHubOptions)
    {
        if (iotHubOptions is null ||
            string.IsNullOrEmpty(iotHubOptions.ConnectionString))
        {
            throw new InvalidOperationException($"Required service '{nameof(IotHubOptions)}' is not registered");
        }

        services.TryAddSingleton<IDeviceTwinModuleExtractor, DeviceTwinModuleExtractor>();

        services.AddSingleton<IIoTHubModuleService, IoTHubModuleService>(s =>
            new IoTHubModuleService(
                iotHubOptions,
                s.GetRequiredService<ILoggerFactory>()));

        services.AddSingleton<IIoTHubService, IoTHubService>(s =>
            new IoTHubService(
                s.GetRequiredService<IIoTHubModuleService>(),
                iotHubOptions,
                s.GetRequiredService<ILoggerFactory>()));

        return services;
    }

    /// <summary>
    /// Configures DPS related services and registers them to the provided IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="deviceProvisioningServiceOptions">The deviceProvisioningServiceOptions.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="DeviceProvisioningServiceOptions"/> service is not registered or the connection string is missing.</exception>
    public static IServiceCollection ConfigureDeviceProvisioningServices(
        this IServiceCollection services,
        DeviceProvisioningServiceOptions deviceProvisioningServiceOptions)
    {
        if (deviceProvisioningServiceOptions is null ||
            string.IsNullOrEmpty(deviceProvisioningServiceOptions.ConnectionString))
        {
            throw new InvalidOperationException($"Required service '{nameof(DeviceProvisioningServiceOptions)}' is not registered");
        }

        services.TryAddSingleton<IConfigurationContentProvider, ConfigurationContentProvider>();

        services.AddSingleton<IDeviceProvisioningService, DeviceProvisioningService>(s => new DeviceProvisioningService(
            deviceProvisioningServiceOptions,
            s.GetRequiredService<ILoggerFactory>()));

        return services;
    }
}