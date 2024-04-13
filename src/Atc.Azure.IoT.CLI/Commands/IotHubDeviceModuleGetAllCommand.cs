namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceModuleGetAllCommand : AsyncCommand<IotHubDeviceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceModuleGetAllCommand> logger;

    public IotHubDeviceModuleGetAllCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceModuleGetAllCommand>();
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

        var modulesOnIotEdgeDevice = await iotHubService.GetModulesOnIotEdgeDevice(
            settings.DeviceId!,
            CancellationToken.None);

        foreach (var module in modulesOnIotEdgeDevice)
        {
            logger.LogInformation("Module:\n" +
                                  $"\t\tDeviceId: {module.DeviceId}\n" +
                                  $"\t\tModuleId: {module.Id}\n" +
                                  $"\t\tConnectionState: {module.ConnectionState}\n" +
                                  $"\t\tConnectionStateUpdatedTime: {module.ConnectionStateUpdatedTime}\n" +
                                  $"\t\tLastActivityTime: {module.LastActivityTime}\n" +
                                  $"\t\tCloudToDeviceMessageCount: {module.CloudToDeviceMessageCount}\n" +
                                  $"\t\tAuthenticationType: {module.Authentication.Type}");
        }

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}