namespace Atc.Azure.IoT.Services;

public abstract class ServiceBase
{
    protected static void ValidateAndAssign(
        string connectionString,
        Action<string> action)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        try
        {
            action(connectionString);
        }
        catch (ArgumentException argumentException)
        {
            throw new InvalidConfigurationException(
                "Invalid service configuration for ConnectionString.",
                argumentException);
        }
        catch (FormatException formatException)
        {
            // The connection string holds the shared access key, so it must not end up in the message.
            throw new InvalidConfigurationException(
                "Invalid service configuration for ConnectionString. Expected the format 'HostName=<hub>.azure-devices.net;SharedAccessKeyName=<policy>;SharedAccessKey=<key>'.",
                formatException);
        }
    }

    protected abstract void Assign(string connectionString);
}