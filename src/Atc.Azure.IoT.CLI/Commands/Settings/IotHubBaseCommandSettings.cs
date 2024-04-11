namespace Atc.Azure.IoT.CLI.Commands.Settings;

public class IotHubBaseCommandSettings : BaseCommandSettings
{
    [CommandOption("-c|--connection-string <CONNECTION-STRING>")]
    [Description("OPC UA Server Url")]
    public string? ConnectionString { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            return ValidationResult.Error("ConnectionString must be present.");
        }

        return ValidationResult.Success();
    }
}