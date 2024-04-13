namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceTwinGetCommand : AsyncCommand<IotHubDeviceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceTwinGetCommand> logger;

    public IotHubDeviceTwinGetCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceTwinGetCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var deviceTwin = await iotHubService.GetDeviceTwin(
            settings.DeviceId!,
            CancellationToken.None);

        if (deviceTwin is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("DeviceTwin:\n" +
                              $"\t\tDeviceId: {deviceTwin.DeviceId}\n" +
                              $"\t\tConnectionState: {deviceTwin.ConnectionState}\n" +
                              $"\t\tStatus: {deviceTwin.Status}\n" +
                              $"\t\tStatusReason: {deviceTwin.StatusReason}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}