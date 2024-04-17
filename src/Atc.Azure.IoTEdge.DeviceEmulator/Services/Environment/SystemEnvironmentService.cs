namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;

/// <summary>
/// The main SystemEnvironmentService - Handles call execution.
/// </summary>
public partial class SystemEnvironmentService : ISystemEnvironmentService
{
    public SystemEnvironmentService(
        ILogger logger)
    {
        this.logger = logger;
    }

    public FileInfo GetCommandFilePath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new FileInfo(Path.Combine(System.Environment.SystemDirectory, "cmd.exe"));
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new FileInfo("/bin/sh");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            const string message = "OSX Platform is not implemented!";
            LogEnvironmentPlatformNotImplemented(message);
            throw new NotImplementedException(message);
        }

        const string notSupportedMessage = "Unknown OS Platform is not supported!";
        LogEnvironmentPlatformNotSupported(notSupportedMessage);
        throw new NotSupportedException(notSupportedMessage);
    }

    public void SetEnvironmentVariables(
        IDictionary<string, string> variables)
    {
        ArgumentNullException.ThrowIfNull(variables);

        foreach (var (key, value) in variables)
        {
            System.Environment.SetEnvironmentVariable(key, value);
            LogEnvironmentInjectedVariable(key, value);
        }
    }
}