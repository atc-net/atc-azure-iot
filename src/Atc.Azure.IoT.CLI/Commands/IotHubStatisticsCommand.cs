namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubStatisticsCommand : AsyncCommand<IotHubBaseCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubStatisticsCommand> logger;

    public IotHubStatisticsCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubStatisticsCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubBaseCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubBaseCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

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