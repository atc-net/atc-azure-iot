namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceModuleRemoveCommand : AsyncCommand<IotHubModuleCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceModuleRemoveCommand> logger;

    public IotHubDeviceModuleRemoveCommand(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        logger = this.loggerFactory.CreateLogger<IotHubDeviceModuleRemoveCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubModuleCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings, cancellationToken);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubModuleCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ConsoleHelper.WriteHeader();

        var deviceId = settings.DeviceId!;
        var moduleId = settings.ModuleId!;

        var iotHubService = IotHubServiceFactory.Create(
            settings.ConnectionString!,
            loggerFactory);

        var sw = Stopwatch.StartNew();

        var removed = await iotHubService.RemoveModuleFromDevice(
            deviceId,
            moduleId,
            cancellationToken);

        if (!removed)
        {
            logger.LogError($"Module '{moduleId}' does not exist on device '{deviceId}'.");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Module '{moduleId}' removed from device '{deviceId}'.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}