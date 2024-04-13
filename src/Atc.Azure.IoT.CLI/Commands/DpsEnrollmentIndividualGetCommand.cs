namespace Atc.Azure.IoT.CLI.Commands;

public sealed class DpsEnrollmentIndividualGetCommand : AsyncCommand<DeviceProvisioningServiceCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<DpsEnrollmentIndividualGetCommand> logger;

    public DpsEnrollmentIndividualGetCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<DpsEnrollmentIndividualGetCommand>();
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

        var individualEnrollment = await dpsService.GetIndividualEnrollment(
            settings.RegistrationId!,
            CancellationToken.None);

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