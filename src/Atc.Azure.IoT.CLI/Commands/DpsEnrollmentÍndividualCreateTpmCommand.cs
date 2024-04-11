namespace Atc.Azure.IoT.CLI.Commands;

public sealed class DpsEnrollmentÍndividualCreateTpmCommand : AsyncCommand<IotHubBaseCommandSettings>
{
    private readonly ILogger<DpsEnrollmentÍndividualCreateTpmCommand> logger;

    public DpsEnrollmentÍndividualCreateTpmCommand(
        ILogger<DpsEnrollmentÍndividualCreateTpmCommand> logger)
    {
        this.logger = logger;
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