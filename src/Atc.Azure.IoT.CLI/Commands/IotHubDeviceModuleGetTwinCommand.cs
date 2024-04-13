namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceModuleGetTwinCommand : AsyncCommand<IotHubModuleCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceModuleGetTwinCommand> logger;

    public IotHubDeviceModuleGetTwinCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceModuleGetTwinCommand>();
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

        var moduleTwin = await iotHubService.GetModuleTwin(
            settings.DeviceId!,
            settings.ModuleId!,
            CancellationToken.None);

        if (moduleTwin is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("ModuleTwin:\n" +
                              $"\t\tDeviceId: {moduleTwin.DeviceId}\n" +
                              $"\t\tModuleId: {moduleTwin.ModuleId}\n" +
                              $"\t\tConnectionState: {moduleTwin.ConnectionState}\n" +
                              $"\t\tStatus: {moduleTwin.Status}\n" +
                              $"\t\tStatusReason: {moduleTwin.StatusReason}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}