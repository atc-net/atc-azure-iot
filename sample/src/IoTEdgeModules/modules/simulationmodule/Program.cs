namespace SimulationModule;

public static class Program
{
    private static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddSimpleConsole(options => options.TimestampFormat = LoggingConstants.TimestampFormat);
                    builder.SetMinimumLevel(LogLevel.Trace);
                });

                if (hostContext.IsStandaloneMode())
                {
                    services.AddSingleton<IModuleClientWrapper, MockModuleClientWrapper>();
                }
                else
                {
                    services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings());
                }

                services.AddSingleton(_ =>
                {
                    var config = new NumberGeneratorConfig()
                        .WithSinHourlyCycle()
                        .WithLowVolatility()
                        .WithMinValue(10)
                        .WithMaxValue(35)
                        .WithMinMaxPolynomialBias(22.5)
                        .WithSinglePointPrecision();

                    return new NumberGenerator(config);
                });

                services.AddQuartz();

                services.AddHostedService<SimulationModuleService>();

                services.Configure<HostOptions>(options =>
                {
                    options.ShutdownTimeout = TimeSpan.FromSeconds(10);
                });
            })
            .UseConsoleLifetime()
            .Build();

        await host.RunAsync();
    }
}