namespace Atc.Azure.IoT.CLI.Commands.Settings;

public class IotHubDeviceCommandSettings : IotHubBaseCommandSettings
{
    [CommandOption("-d|--device-id <DEVICE-ID>")]
    [Description("IotHub Device Id")]
    public string? DeviceId { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(DeviceId))
        {
            return ValidationResult.Error("DeviceId must be present.");
        }

        return ValidationResult.Success();
    }
}