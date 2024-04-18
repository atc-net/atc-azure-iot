namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class IotHubDeviceCreateCommandSettings : IotHubDeviceCommandSettings
{
    [CommandOption("--edge-device <EDGE-DEVICE>")]
    [Description("Indicates if only edge devices should be queried.")]
    [DefaultValue(false)]
    public bool EdgeDevice { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        return ValidationResult.Success();
    }
}