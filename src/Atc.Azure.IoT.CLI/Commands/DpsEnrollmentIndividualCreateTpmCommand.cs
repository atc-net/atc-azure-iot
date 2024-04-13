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
            ParseToDictionary(settings.Tags?.Value ?? null),
            ParseToDictionary(settings.DesiredProperties?.Value ?? null),
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

    private static Dictionary<string, string> ParseToDictionary(
        string? input)
    {
        var dictionary = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(input))
        {
            return dictionary;
        }

        var pairs = input.Split(',');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                dictionary[keyValue[0]] = keyValue[1];
            }
        }

        return dictionary;
    }
}