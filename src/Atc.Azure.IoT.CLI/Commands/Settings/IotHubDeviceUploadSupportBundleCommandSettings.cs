namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class IotHubDeviceUploadSupportBundleCommandSettings : IotHubDeviceCommandSettings
{
    [CommandOption("--sas-url <SAS-URL>")]
    public string? SasUrl { get; init; }

    [CommandOption("--since [SINCE]")]
    public FlagValue<string>? Since { get; init; }

    [CommandOption("--until [UNTIL]")]
    public FlagValue<string>? Until { get; init; }

    [CommandOption("--include-edge-runtime-only <INCLUDE-EDGE-RUNTIME-ONLY>")]
    [DefaultValue(false)]
    public bool IncludeEdgeRuntimeOnly { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrWhiteSpace(SasUrl))
        {
            return ValidationResult.Error($"{nameof(SasUrl)} must be present.");
        }

        if (!Uri.TryCreate(SasUrl, UriKind.Absolute, out _))
        {
            return ValidationResult.Error($"{nameof(SasUrl)} must be a valid absolute URI)");
        }

        return ValidationResult.Success();
    }
}