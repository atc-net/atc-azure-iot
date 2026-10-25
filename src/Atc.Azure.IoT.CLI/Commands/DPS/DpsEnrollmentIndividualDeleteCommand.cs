namespace Atc.Azure.IoT.CLI.Commands.DPS;

public sealed class DpsEnrollmentIndividualDeleteCommand : AsyncCommand<DpsCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<DpsEnrollmentIndividualDeleteCommand> logger;

    public DpsEnrollmentIndividualDeleteCommand(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        logger = this.loggerFactory.CreateLogger<DpsEnrollmentIndividualDeleteCommand>();
    }

    public override Task<int> ExecuteAsync(
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

        var succeeded = await dpsService.DeleteIndividualEnrollment(
            settings.RegistrationId!,
            cancellationToken);

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