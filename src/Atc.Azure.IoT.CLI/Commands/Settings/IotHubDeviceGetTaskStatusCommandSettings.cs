namespace Atc.Azure.IoT.CLI.Commands.Settings;

public sealed class IotHubDeviceGetTaskStatusCommandSettings : IotHubDeviceCommandSettings
{
    [CommandOption("--correlation-id <CORRELATION-ID>")]
    public string? CorrelationId { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrWhiteSpace(CorrelationId))
        {
            return ValidationResult.Error($"{nameof(CorrelationId)} must be present.");
        }

        return ValidationResult.Success();
    }
}