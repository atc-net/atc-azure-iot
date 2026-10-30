namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceCreateCommand : AsyncCommand<IotHubDeviceCreateCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceCreateCommand> logger;

    public IotHubDeviceCreateCommand(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        logger = this.loggerFactory.CreateLogger<IotHubDeviceCreateCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceCreateCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings, cancellationToken);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceCreateCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ConsoleHelper.WriteHeader();

        var deviceId = settings.DeviceId!;
        var iotHubService = IotHubServiceFactory.Create(
            settings.ConnectionString!,
            loggerFactory);

        var sw = Stopwatch.StartNew();

        await iotHubService.CreateDevice(
            deviceId,
            settings.EdgeEnabled,
            cancellationToken);

        logger.LogInformation($"Device with id '{deviceId}' was created successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}