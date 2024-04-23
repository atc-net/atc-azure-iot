namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class DpsEnrollmentIndividualCreateTpmCommandSettings : DpsCommandSettings
{
    [CommandOption("--endorsement-key <ENDORSEMENT-KEY>")]
    [Description("Endorsement Key")]
    public string? EndorsementKey { get; init; }

    [CommandOption("-d|--device-id <DEVICE-ID>")]
    [Description("Device Id")]
    public string? DeviceId { get; init; }

    [CommandOption("--tags [TAGS]")]
    [Description("Tags")]
    public FlagValue<string>? Tags { get; init; }

    [CommandOption("--desired-properties [DESIRED-PROPERTIES]")]
    [Description("Desired Properties")]
    public FlagValue<string>? DesiredProperties { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrWhiteSpace(RegistrationId))
        {
            return ValidationResult.Error($"{nameof(RegistrationId)} must be present.");
        }

        if (string.IsNullOrWhiteSpace(DeviceId))
        {
            return ValidationResult.Error($"{nameof(DeviceId)} must be present.");
        }

        return ValidationResult.Success();
    }
}