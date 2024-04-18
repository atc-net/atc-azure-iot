namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceConnectionStringGetCommand : AsyncCommand<IotHubDeviceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceConnectionStringGetCommand> logger;

    public IotHubDeviceConnectionStringGetCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceConnectionStringGetCommand>();
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

        var deviceConnectionString = await iotHubService.GetDeviceConnectionString(
            deviceId,
            CancellationToken.None);

        if (string.IsNullOrEmpty(deviceConnectionString))
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Retrieved connection-string from device with id '{deviceId}' was created successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}