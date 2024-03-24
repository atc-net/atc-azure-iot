namespace Atc.Azure.IoT.Exceptions;

public sealed class InvalidConfigurationException : Exception
{
    private const string ExceptionMessage = "Invalid configuration passed to a service.";

    public InvalidConfigurationException()
        : base(ExceptionMessage)
    {
    }

    public InvalidConfigurationException(string message)
        : base(message)
    {
    }

    public InvalidConfigurationException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}