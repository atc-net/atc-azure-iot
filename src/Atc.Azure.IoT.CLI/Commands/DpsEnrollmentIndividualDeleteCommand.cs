namespace Atc.Azure.IoT.CLI.Commands;

public sealed class DpsEnrollmentIndividualDeleteCommand : AsyncCommand<DeviceProvisioningServiceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<DpsEnrollmentIndividualDeleteCommand> logger;

    public DpsEnrollmentIndividualDeleteCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<DpsEnrollmentIndividualDeleteCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        DeviceProvisioningServiceCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        DeviceProvisioningServiceCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var dpsService = DeviceProvisioningServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var succeeded = await dpsService.DeleteIndividualEnrollment(
            settings.RegistrationId!,
            CancellationToken.None);

        if (!succeeded)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Individual enrollment with registration id '{settings.RegistrationId}' was deleted successfully.");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}