namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceTwinUpdateCommand : AsyncCommand<IotHubBaseCommandSettings>
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
        IotHubBaseCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubBaseCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var connectionString = settings.ConnectionString!;
        var sw = Stopwatch.StartNew();

        // TODO:

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}