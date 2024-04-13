namespace Atc.Azure.IoT.CLI.Commands;

public sealed class IotHubDeviceTwinGetAllCommand : AsyncCommand<IotHubBaseCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceTwinGetAllCommand> logger;

    public IotHubDeviceTwinGetAllCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceTwinGetAllCommand>();
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