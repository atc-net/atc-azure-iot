namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceGetCommand : AsyncCommand<IotHubDeviceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceGetCommand> logger;

    public IotHubDeviceGetCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceGetCommand>();
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

        var deviceId = settings.DeviceId!;
        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var device = await iotHubService.GetDevice(deviceId, CancellationToken.None);
        if (device is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Device:\n" +
                              $"\t\tId: {device.Id}\n" +
                              $"\t\tConnectionState: {device.ConnectionState}\n" +
                              $"\t\tStatus: {device.Status}\n" +
                              $"\t\tStatusReason: {device.StatusReason}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}