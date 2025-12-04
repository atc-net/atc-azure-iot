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
    /// <param name="loggerFactoryResolver">
    /// Optional resolver for <see cref="ILoggerFactory"/>. If not provided, uses DI's ILoggerFactory if registered.
    /// Use <c>_ => null</c> to explicitly disable logging.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="IotHubOptions"/> service is not registered or the connection string is missing.</exception>
    public static IServiceCollection ConfigureIotHubServices(
        this IServiceCollection services,
        IotHubOptions iotHubOptions,
        Func<IServiceProvider, ILoggerFactory?>? loggerFactoryResolver = null)
    {
        if (iotHubOptions is null ||
            string.IsNullOrEmpty(iotHubOptions.ConnectionString))
        {
            throw new InvalidOperationException($"Required service '{nameof(IotHubOptions)}' is not registered");
        }

        services.TryAddSingleton<IDeviceTwinModuleExtractor, DeviceTwinModuleExtractor>();

        services.AddSingleton<IIoTHubModuleService, IoTHubModuleService>(sp =>
            new IoTHubModuleService(
                iotHubOptions,
                ResolveLoggerFactory(sp, loggerFactoryResolver)));

        services.AddSingleton<IIoTHubService, IoTHubService>(sp =>
            new IoTHubService(
                sp.GetRequiredService<IIoTHubModuleService>(),
                iotHubOptions,
                ResolveLoggerFactory(sp, loggerFactoryResolver)));

        return services;
    }

    /// <summary>
    /// Configures DPS related services and registers them to the provided IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="deviceProvisioningServiceOptions">The deviceProvisioningServiceOptions.</param>
    /// <param name="loggerFactoryResolver">
    /// Optional resolver for <see cref="ILoggerFactory"/>. If not provided, uses DI's ILoggerFactory if registered.
    /// Use <c>_ => null</c> to explicitly disable logging.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="DeviceProvisioningServiceOptions"/> service is not registered or the connection string is missing.</exception>
    public static IServiceCollection ConfigureDeviceProvisioningServices(
        this IServiceCollection services,
        DeviceProvisioningServiceOptions deviceProvisioningServiceOptions,
        Func<IServiceProvider, ILoggerFactory?>? loggerFactoryResolver = null)
    {
        if (deviceProvisioningServiceOptions is null ||
            string.IsNullOrEmpty(deviceProvisioningServiceOptions.ConnectionString))
        {
            throw new InvalidOperationException($"Required service '{nameof(DeviceProvisioningServiceOptions)}' is not registered");
        }

        services.TryAddSingleton<IConfigurationContentProvider, ConfigurationContentProvider>();

        services.AddSingleton<IDeviceProvisioningService, DeviceProvisioningService>(sp =>
            new DeviceProvisioningService(
                deviceProvisioningServiceOptions,
                ResolveLoggerFactory(sp, loggerFactoryResolver)));

        return services;
    }

    private static ILoggerFactory? ResolveLoggerFactory(
        IServiceProvider serviceProvider,
        Func<IServiceProvider, ILoggerFactory?>? loggerFactoryResolver)
        => loggerFactoryResolver is not null
            ? loggerFactoryResolver(serviceProvider)
            : serviceProvider.GetService<ILoggerFactory>();
}