namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;

/// <summary>
/// The main SystemEnvironmentService - Handles call execution.
/// </summary>
public partial class SystemEnvironmentService : ISystemEnvironmentService
{
    public SystemEnvironmentService(
        ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<SystemEnvironmentService>();
    }

    public FileInfo GetCommandFilePath()
    {
        if (OperatingSystem.IsWindows())
        {
            return new FileInfo(Path.Combine(System.Environment.SystemDirectory, "cmd.exe"));
        }

        if (OperatingSystem.IsLinux())
        {
            return new FileInfo("/bin/sh");
        }

        if (OperatingSystem.IsMacOS())
        {
            const string message = "MacOS Platform is not implemented!";
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