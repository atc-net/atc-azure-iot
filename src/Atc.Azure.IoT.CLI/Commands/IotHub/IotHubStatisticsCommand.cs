namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubStatisticsCommand : AsyncCommand<ConnectionBaseCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubStatisticsCommand> logger;

    public IotHubStatisticsCommand(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        logger = this.loggerFactory.CreateLogger<IotHubStatisticsCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ConnectionBaseCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(ConnectionBaseCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var iotHubService = IotHubServiceFactory.Create(
            settings.ConnectionString!,
            loggerFactory);

        var sw = Stopwatch.StartNew();

        var deviceRegistryStatistics = await iotHubService.GetDeviceRegistryStatistics(CancellationToken.None);
        if (deviceRegistryStatistics is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("RegistryStatistics:\n" +
                              $"\t\tTotalDeviceCount: {deviceRegistryStatistics.TotalDeviceCount}\n" +
                              $"\t\tEnabledDeviceCount: {deviceRegistryStatistics.EnabledDeviceCount}\n" +
                              $"\t\tDisabledDeviceCount: {deviceRegistryStatistics.DisabledDeviceCount}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}