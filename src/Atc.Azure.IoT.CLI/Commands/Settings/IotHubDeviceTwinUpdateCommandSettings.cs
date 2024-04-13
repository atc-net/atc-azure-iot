namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class IotHubDeviceTwinUpdateCommandSettings : IotHubDeviceCommandSettings
{
    [CommandOption("--tags <TAGS>")]
    [Description("Tags")]
    public string? Tags { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrWhiteSpace(Tags))
        {
            return ValidationResult.Error($"{nameof(Tags)} must be present.");
        }

        return ValidationResult.Success();
    }
}