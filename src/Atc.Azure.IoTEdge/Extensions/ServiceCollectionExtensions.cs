namespace Atc.Azure.IoTEdge.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModuleClientWrapper(
        this IServiceCollection serviceCollection,
        ITransportSettings transportSettings)
        => serviceCollection.AddSingleton<IModuleClientWrapper>(_ =>
        {
            ITransportSettings[] settings = [transportSettings];

            var ioTHubModuleClient = ModuleClient
                .CreateFromEnvironmentAsync(settings)
                .GetAwaiter()
                .GetResult();

            return new ModuleClientWrapper(ioTHubModuleClient);
        });
}