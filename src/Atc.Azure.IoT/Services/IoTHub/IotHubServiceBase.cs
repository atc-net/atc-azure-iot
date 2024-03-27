namespace Atc.Azure.IoT.Services.IoTHub;

public abstract class IotHubServiceBase
{
    protected static void ValidateAndAssign(
        string iotHubConnectionString,
        Action<string> action)
    {
        ArgumentException.ThrowIfNullOrEmpty(iotHubConnectionString);

        try
        {
            action(iotHubConnectionString);
        }
        catch (ArgumentException argumentException)
        {
            throw new InvalidConfigurationException(
                "Invalid service configuration for IoTHubConnectionString.",
                argumentException);
        }
        catch (FormatException formatException)
        {
            throw new InvalidConfigurationException(
                $"Invalid service configuration for IoTHubConnectionString: {iotHubConnectionString}",
                formatException);
        }
    }

    protected abstract void Assign(
        string iotHubConnectionString);
}