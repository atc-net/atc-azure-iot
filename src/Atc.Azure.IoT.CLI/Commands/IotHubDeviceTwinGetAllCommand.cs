namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceTwinGetAllCommand : AsyncCommand<IotHubDeviceTwinGetAllCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceTwinGetAllCommand> logger;

    public IotHubDeviceTwinGetAllCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceTwinGetAllCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceTwinGetAllCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceTwinGetAllCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var deviceTwins = await iotHubService.GetDeviceTwins(settings.OnlyIncludeEdgeDevices);
        foreach (var deviceTwin in deviceTwins)
        {
            logger.LogInformation("DeviceTwin:\n" +
                                  $"\t\tDeviceId: {deviceTwin.DeviceId}\n" +
                                  $"\t\tConnectionState: {deviceTwin.ConnectionState}\n" +
                                  $"\t\tStatus: {deviceTwin.Status}\n" +
                                  $"\t\tStatusReason: {deviceTwin.StatusReason}");
        }

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}