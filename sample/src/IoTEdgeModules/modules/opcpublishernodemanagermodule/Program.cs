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
                    // The IoT Edge runtime, or the Aspire AppHost, gives the module the CA of edgeHub's certificate.
                    // The AppHost can't on macOS, so a module running there accepts the test certificate instead.
                    var certificateValidation = OperatingSystem.IsMacOS()
                        ? ServerCertificateValidation.AcceptAny
                        : ServerCertificateValidation.Validate;

                    services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings(certificateValidation));
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