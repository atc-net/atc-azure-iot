namespace Atc.Azure.IoT.CLI.Commands.Settings;

public class IotHubDeviceCommandSettings : ConnectionBaseCommandSettings
{
    [CommandOption("-d|--device-id <DEVICE-ID>")]
    [Description("IotHub Device Id")]
    public string? DeviceId { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrWhiteSpace(DeviceId))
        {
            return ValidationResult.Error($"{nameof(DeviceId)} must be present.");
        }

        return ValidationResult.Success();
    }
}