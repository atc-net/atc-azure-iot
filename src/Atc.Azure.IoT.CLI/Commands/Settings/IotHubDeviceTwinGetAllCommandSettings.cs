namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class IotHubDeviceTwinGetAllCommandSettings : ConnectionBaseCommandSettings
{
    [CommandOption("--edge-devices-only <EDGE-DEVICES-ONLY>")]
    [Description("Indicates if only edge devices should be queried.")]
    [DefaultValue(false)]
    public bool OnlyIncludeEdgeDevices { get; init; }

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