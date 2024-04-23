namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class IotHubDeviceCreateCommandSettings : IotHubDeviceCommandSettings
{
    [CommandOption("--edge-enabled <EDGE-ENABLED>")]
    [Description("Indicates if the device should be edge enabled.")]
    [DefaultValue(false)]
    public bool EdgeEnabled { get; init; }

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