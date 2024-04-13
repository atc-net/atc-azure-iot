namespace Atc.Azure.IoT.CLI.Commands.Settings;

public class IotHubModuleCommandSettings : IotHubDeviceCommandSettings
{
    [CommandOption("-m|--module-id <MODULE-ID>")]
    [Description("IotHub Module Id")]
    public string? ModuleId { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrWhiteSpace(ModuleId))
        {
            return ValidationResult.Error("ModuleId must be present.");
        }

        return ValidationResult.Success();
    }
}