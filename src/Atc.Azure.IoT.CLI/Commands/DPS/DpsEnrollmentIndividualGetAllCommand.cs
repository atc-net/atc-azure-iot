namespace Atc.Azure.IoT.CLI.Commands.DPS;

public sealed class DpsEnrollmentIndividualGetAllCommand : AsyncCommand<ConnectionBaseCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<DpsEnrollmentIndividualGetAllCommand> logger;

    public DpsEnrollmentIndividualGetAllCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<DpsEnrollmentIndividualGetAllCommand>();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ConnectionBaseCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        ConnectionBaseCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var dpsService = DeviceProvisioningServiceFactory.Create(
            loggerFactory,
            settings.ConnectionString!);

        var sw = Stopwatch.StartNew();

        var individualEnrollments = await dpsService.GetIndividualEnrollments(CancellationToken.None);

        foreach (var individualEnrollment in individualEnrollments)
        {
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
        }

        sw.Stop();
        logger.LogDebug($"Time for operation: {sw.Elapsed.GetPrettyTime()}");

        return ConsoleExitStatusCodes.Success;
    }
}