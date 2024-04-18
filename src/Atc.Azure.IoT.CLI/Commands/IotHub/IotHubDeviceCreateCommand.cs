namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceCreateCommand : AsyncCommand<IotHubDeviceCreateCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceCreateCommand> logger;

    public IotHubDeviceCreateCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceCreateCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceCreateCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceCreateCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var deviceId = settings.DeviceId!;
        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var (succeeded, device) = await iotHubService.CreateDevice(
            deviceId,
            settings.EdgeDevice,
            CancellationToken.None);

        if (!succeeded ||
            device is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Device with id '{deviceId}' was created successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}