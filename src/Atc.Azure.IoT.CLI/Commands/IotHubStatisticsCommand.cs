namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubStatisticsCommand : AsyncCommand<SingleNodeCommandSettings>
{
    private readonly ILogger<IotHubStatisticsCommand> logger;

    public IotHubStatisticsCommand(
        ILogger<IotHubStatisticsCommand> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        SingleNodeCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        SingleNodeCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var serverUrl = new Uri(settings.ServerUrl!);
        var userName = settings.UserName;
        var password = settings.Password;
        var nodeId = settings.NodeId;
        var includeSampleValue = settings.IncludeSampleValue;

        var sw = Stopwatch.StartNew();

        // TODO:

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}