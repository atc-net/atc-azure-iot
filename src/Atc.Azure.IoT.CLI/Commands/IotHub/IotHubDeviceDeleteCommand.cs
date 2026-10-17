namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceDeleteCommand : AsyncCommand<IotHubDeviceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceDeleteCommand> logger;

    public IotHubDeviceDeleteCommand(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        logger = this.loggerFactory.CreateLogger<IotHubDeviceDeleteCommand>();
    }

    protected override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings, cancellationToken);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ConsoleHelper.WriteHeader();

        var deviceId = settings.DeviceId!;
        var iotHubService = IotHubServiceFactory.Create(
            settings.ConnectionString!,
            loggerFactory);

        var sw = Stopwatch.StartNew();

        var succeeded = await iotHubService.DeleteDevice(
            deviceId,
            cancellationToken);

        if (!succeeded)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Device with id '{deviceId}' was deleted successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}