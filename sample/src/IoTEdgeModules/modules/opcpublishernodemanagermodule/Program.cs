namespace OpcPublisherNodeManagerModule;

public static class Program
{
    private static ILogger? logger;
    private static ILoggerFactory? loggerFactory;
    private static IoTEdgeEmulationService? emulationService;

    private static async Task Main(string[] args)
    {
        var isEmulatorMode = false;

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
                    if (hostContext.IsEmulatorMode())
                    {
                        isEmulatorMode = true;

                        services.ConfigureEmulatorOptions(hostContext.Configuration);
                        services.AddModuleClientWrapper(TransportSettingsFactory.BuildAmqpTransportSettings());

                        // Creating a temporary scope to resolve EmulatorOptions
                        var serviceProvider = services.BuildServiceProvider();
                        using var scope = serviceProvider.CreateScope();
                        var emulatorOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmulatorOptions>>().Value;
                        var iotHubOptions = new IotHubOptions
                        {
                            ConnectionString = emulatorOptions.IotHubConnectionString,
                        };

                        services.ConfigureIotHubServices(iotHubOptions);
                    }
                    else
                    {
                        services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings());
                    }
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

        loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        logger = loggerFactory.CreateLogger<OpcPublisherNodeManagerModuleService>();

        if (isEmulatorMode)
        {
            await SetupEmulator(host.Services);
        }

        await host.RunAsync();
    }

    private static async Task SetupEmulator(
        IServiceProvider serviceProvider)
    {
        Console.CancelKeyPress += OnEmulatorCancelKeyPress;
        AppDomain.CurrentDomain.UnhandledException += (_, e) => OnEmulatorUnhandledException(e);

        var emulatorOptions = serviceProvider.GetRequiredService<EmulatorOptions>();
        var systemEnvironmentService = new SystemEnvironmentService(loggerFactory!);

        var dockerService = new DockerService(loggerFactory!, systemEnvironmentService);
        emulationService = IoTEdgeEmulationServiceFactory.BuildIoTEdgeEmulationService(
            loggerFactory!,
            dockerService,
            serviceProvider.GetRequiredService<IIoTHubService>());

        var isEmulatorStarted = await emulationService
            .StartEmulator(
                emulatorOptions.TemplateFilePath,
                emulatorOptions.IotHubConnectionString,
                deviceId: string.Empty,
                containerName: string.Empty,
                CancellationToken.None);

        if (!isEmulatorStarted)
        {
            throw new PreconditionFailedException("Could not start emulator!");
        }

        var isEdgeEmulatorReady = await dockerService.EnsureIotEdgeEmulatorIsReady(OpcPublisherNodeManagerModuleConstants.ModuleId);
        if (!isEdgeEmulatorReady)
        {
            throw new PreconditionFailedException("Emulator containers are not ready!");
        }

        var (getVariablesSucceeded, iotEdgeVariables) = await dockerService.GetIotEdgeVariablesFromContainer(OpcPublisherNodeManagerModuleConstants.ModuleId);
        if (!getVariablesSucceeded)
        {
            throw new PreconditionFailedException("Could not retrieve emulator IoTEdge variables!");
        }

        emulationService.EnsureProperIotEdgeEnvironmentVariables(iotEdgeVariables!);
        systemEnvironmentService.SetEnvironmentVariables(iotEdgeVariables!);
    }

    private static void OnEmulatorCancelKeyPress(
        object? sender,
        ConsoleCancelEventArgs args)
    {
        logger!.LogInformation("Stop Emulation - cancelled by user.");
        if (emulationService is not null)
        {
            TaskHelper.RunSync(async () => await emulationService.StopEmulator(CancellationToken.None));
        }
    }

    private static void OnEmulatorUnhandledException(
        UnhandledExceptionEventArgs args)
    {
        if (logger is null)
        {
            Console.WriteLine($"Unhandled Exception: {args.ExceptionObject}");
        }
        else
        {
            logger.LogInformation($"Unhandled Exception: {args.ExceptionObject}");
        }
    }
}