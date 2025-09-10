namespace Atc.Azure.IoT.CLI.Commands.IotHub;

public sealed class IotHubDeviceUploadSupportBundleCommand : AsyncCommand<IotHubDeviceUploadSupportBundleCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<IotHubDeviceUploadSupportBundleCommand> logger;

    public IotHubDeviceUploadSupportBundleCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<IotHubDeviceUploadSupportBundleCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        IotHubDeviceUploadSupportBundleCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        IotHubDeviceUploadSupportBundleCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var iotHubService = IotHubServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var result = await iotHubService.UploadSupportBundle(
            settings.DeviceId!,
            new UploadSupportBundleRequest(
                new Uri(settings.SasUrl!),
                settings.Since is { IsSet: true } ? settings.Since.Value : string.Empty,
                settings.Until is { IsSet: true } ? settings.Until.Value : string.Empty,
                settings.IncludeEdgeRuntimeOnly),
            CancellationToken.None);

        if (result.StatusCode != StatusCodes.Status200OK)
        {
            logger.LogError($"Failed to initiate support bundle upload on device - StatusCode: {result.StatusCode} - Response: {result.Payload}");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Upload support bundle request started successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        logger.LogInformation("{Payload}", result.Payload);

        return ConsoleExitStatusCodes.Success;
    }
}