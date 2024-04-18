namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceModuleRestartCommand : AsyncCommand<IotHubModuleCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceModuleRestartCommand> logger;

    public IotHubDeviceModuleRestartCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceModuleRestartCommand>();
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

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var (succeeded, statusCode, jsonPayload) = await iotHubService.RestartModuleOnDevice(
            settings.DeviceId!,
            settings.ModuleId!,
            CancellationToken.None);

        if (!succeeded)
        {
            logger.LogError($"Failed to restart module on device - StatusCode: {statusCode} - Response: {jsonPayload}");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Module restarted successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}