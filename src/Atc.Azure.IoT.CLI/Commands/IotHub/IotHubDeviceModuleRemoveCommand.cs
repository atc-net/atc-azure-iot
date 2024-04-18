namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceModuleRemoveCommand : AsyncCommand<IotHubModuleCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceModuleRemoveCommand> logger;

    public IotHubDeviceModuleRemoveCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceModuleRemoveCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubModuleCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubModuleCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var deviceId = settings.DeviceId!;
        var moduleId = settings.ModuleId!;

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var succeeded = await iotHubService.RemoveModuleFromDevice(
            deviceId,
            moduleId,
            CancellationToken.None);

        if (!succeeded)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Module '{moduleId}' removed from device '{deviceId}'.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}