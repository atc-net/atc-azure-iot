namespace Atc.Azure.IoT.Services.DeviceProvisioning;

// TODO: Merge with  IotHubServiceBase ????
public abstract class DeviceProvisioningServiceBase
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
            throw new InvalidConfigurationException(
                $"Invalid service configuration for ConnectionString: {connectionString}",
                formatException);
        }
    }

    protected abstract void Assign(
        string connectionString);
}