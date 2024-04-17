namespace Atc.Azure.IoTEdge.DeviceEmulator.Options;

public static class EmulatorOptionsExtensions
{
    public static IServiceCollection ConfigureEmulatorOptions(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<EmulatorOptions>(options => config.GetRequiredSection(nameof(EmulatorOptions)).Bind(options));

        services.AddSingleton(s =>
        {
            var options = s.GetRequiredService<IOptions<EmulatorOptions>>().Value;
            return options;
        });

        return services;
    }
}