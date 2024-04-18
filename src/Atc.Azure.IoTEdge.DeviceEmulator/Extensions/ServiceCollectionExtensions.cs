namespace Atc.Azure.IoTEdge.DeviceEmulator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureEmulatorOptions(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<EmulatorOptions>(options => config.GetRequiredSection(nameof(EmulatorOptions)).Bind(options));

        services.AddSingleton(s => s.GetRequiredService<IOptions<EmulatorOptions>>().Value);

        return services;
    }
}