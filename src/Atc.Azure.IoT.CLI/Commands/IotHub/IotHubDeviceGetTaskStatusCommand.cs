namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceGetTaskStatusCommand : AsyncCommand<IotHubDeviceGetTaskStatusCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceGetTaskStatusCommand> logger;

    public IotHubDeviceGetTaskStatusCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceGetTaskStatusCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceGetTaskStatusCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceGetTaskStatusCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var result = await iotHubService.GetTaskStatus(
            settings.DeviceId!,
            new GetTaskStatusRequest(settings.CorrelationId!),
            CancellationToken.None);

        if (result.StatusCode != StatusCodes.Status200OK)
        {
            logger.LogError($"Failed to initiate support bundle upload on device - StatusCode: {result.StatusCode} - Response: {result.Payload}");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Get Task Status executed successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        logger.LogInformation("{Payload}", result.Payload);

        return ConsoleExitStatusCodes.Success;
    }
}