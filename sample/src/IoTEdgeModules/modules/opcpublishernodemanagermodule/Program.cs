namespace OpcPublisherNodeManagerModule;

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

                services.AddSingleton<IMethodResponseFactory, MethodResponseFactory>();
                services.AddHostedService<OpcPublisherNodeManagerModuleService>();

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