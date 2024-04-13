namespace Atc.Azure.IoT.CLI.Commands;

public sealed class DpsEnrollmentIndividualCreateTpmCommand : AsyncCommand<DpsEnrollmentIndividualCreateTpmCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<DpsEnrollmentIndividualCreateTpmCommand> logger;

    public DpsEnrollmentIndividualCreateTpmCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<DpsEnrollmentIndividualCreateTpmCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        DpsEnrollmentIndividualCreateTpmCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        DpsEnrollmentIndividualCreateTpmCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var dpsService = DeviceProvisioningServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var (enrollment, _) = await dpsService.CreateIndividualTpmEnrollment(
            settings.EndorsementKey!,
            settings.RegistrationId!,
            settings.DeviceId!,
            settings.Tags?.Value.ParseToDictionary() ?? null,
            settings.DesiredProperties?.Value.ParseToDictionary() ?? null,
            CancellationToken.None);

        if (enrollment is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"Individual TPM Enrollment created with registration id: {enrollment.RegistrationId}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}