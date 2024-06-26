namespace Atc.Azure.IoT.CLI.Commands.Settings;

public class DpsCommandSettings : ConnectionBaseCommandSettings
{
    [CommandOption("-r|--registration-id <REGISTRATION-ID>")]
    [Description("Registration Id")]
    public string? RegistrationId { get; init; }

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

        return ValidationResult.Success();
    }
}