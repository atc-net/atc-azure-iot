namespace Atc.Azure.IoTEdge.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="IModuleClientWrapper"/> that connects to edgeHub with the given transport settings, using
    /// the IoT Edge environment variables (<see cref="ModuleClient.CreateFromEnvironmentAsync(ITransportSettings[], ClientOptions)"/>).
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="transportSettings">The transport settings, e.g. from <see cref="Factories.TransportSettingsFactory"/>.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddModuleClientWrapper(
        this IServiceCollection serviceCollection,
        ITransportSettings transportSettings)
        => serviceCollection.AddModuleClientWrapper(_ => transportSettings);

    /// <summary>
    /// Registers an <see cref="IModuleClientWrapper"/> that connects to edgeHub with transport settings created when the
    /// module client is, so they can use services, for example the logger that
    /// <see cref="ServerCertificateValidation.Audit"/> needs.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="transportSettingsFactory">Creates the transport settings from the service provider.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddModuleClientWrapper(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, ITransportSettings> transportSettingsFactory)
    {
        ArgumentNullException.ThrowIfNull(transportSettingsFactory);

        return serviceCollection.AddSingleton<IModuleClientWrapper>(serviceProvider =>
        {
            ITransportSettings[] settings = [transportSettingsFactory(serviceProvider)];

            var ioTHubModuleClient = ModuleClient
                .CreateFromEnvironmentAsync(settings)
                .GetAwaiter()
                .GetResult();

            return new ModuleClientWrapper(ioTHubModuleClient);
        });
    }
}