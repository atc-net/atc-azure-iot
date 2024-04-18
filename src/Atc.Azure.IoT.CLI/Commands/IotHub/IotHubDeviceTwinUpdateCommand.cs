namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceTwinUpdateCommand : AsyncCommand<IotHubDeviceTwinUpdateCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceTwinUpdateCommand> logger;

    public IotHubDeviceTwinUpdateCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceTwinUpdateCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceTwinUpdateCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceTwinUpdateCommandSettings settings)
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

        deviceTwin.Tags = settings.Tags.ParseToDictionary().ToTwinCollection();

        var (succeeded, updatedTwin) = await iotHubService.UpdateDeviceTwin(
            settings.DeviceId!,
            deviceTwin,
            CancellationToken.None);

        if (!succeeded ||
            updatedTwin is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("DeviceTwin updated:\n" +
                              $"\t\tDeviceId: {updatedTwin.DeviceId}\n" +
                              $"\t\tConnectionState: {updatedTwin.ConnectionState}\n" +
                              $"\t\tStatus: {updatedTwin.Status}\n" +
                              $"\t\tStatusReason: {updatedTwin.StatusReason}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}