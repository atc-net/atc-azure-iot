namespace Atc.Azure.IoT.CLI.Commands.DPS;

public sealed class DpsEnrollmentIndividualGetCommand : AsyncCommand<DpsCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<DpsEnrollmentIndividualGetCommand> logger;

    public DpsEnrollmentIndividualGetCommand(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        logger = this.loggerFactory.CreateLogger<DpsEnrollmentIndividualGetCommand>();
    }

    protected override Task<int> ExecuteAsync(
        CommandContext context,
        DpsCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings, cancellationToken);
    }

    private async Task<int> ExecuteInternalAsync(
        DpsCommandSettings settings,
        CancellationToken cancellationToken)
    {
        ConsoleHelper.WriteHeader();

        var dpsService = DeviceProvisioningServiceFactory.Create(
            settings.ConnectionString!,
            loggerFactory);

        var sw = Stopwatch.StartNew();

        var individualEnrollment = await dpsService.GetIndividualEnrollment(
            settings.RegistrationId!,
            cancellationToken);

        if (individualEnrollment is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("IndividualEnrollment:\n" +
                              $"\t\tRegistrationId: {individualEnrollment.RegistrationId}\n" +
                              $"\t\tDeviceId: {individualEnrollment.DeviceId}\n" +
                              $"\t\tIotHubHostName: {individualEnrollment.IotHubHostName}\n" +
                              $"\t\tProvisioningStatus: {individualEnrollment.ProvisioningStatus}\n" +
                              $"\t\tCreatedDateTimeUtc: {individualEnrollment.CreatedDateTimeUtc}\n" +
                              $"\t\tLastUpdatedDateTimeUtc: {individualEnrollment.LastUpdatedDateTimeUtc}\n" +
                              $"\t\tLastUpdatedDateTimeUtc: {individualEnrollment.LastUpdatedDateTimeUtc}\n" +
                              $"\t\tAssignedHub: {individualEnrollment.RegistrationState.AssignedHub}\n" +
                              $"\t\tEnrollmentStatus: {individualEnrollment.RegistrationState.Status}");

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}