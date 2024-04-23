namespace Atc.Azure.IoT.CLI.Commands.Settings;

public class ConnectionBaseCommandSettings : BaseCommandSettings
{
    [CommandOption("-c|--connection-string <CONNECTION-STRING>")]
    [Description("ConnectionString")]
    public string? ConnectionString { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            return ValidationResult.Error($"{nameof(ConnectionString)} must be present.");
        }

        return ValidationResult.Success();
    }
}