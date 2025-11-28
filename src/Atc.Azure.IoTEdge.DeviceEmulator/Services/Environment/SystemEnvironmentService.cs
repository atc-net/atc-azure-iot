namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Environment;

/// <summary>
/// The main SystemEnvironmentService - Handles call execution.
/// </summary>
public partial class SystemEnvironmentService : ISystemEnvironmentService
{
    public SystemEnvironmentService(ILoggerFactory? loggerFactory = null)
    {
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<SystemEnvironmentService>()
            : NullLogger<SystemEnvironmentService>.Instance;
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

        var notSupportedMessage = $"Platform {RuntimeInformation.OSDescription} is not supported!";
        LogEnvironmentPlatformNotSupported(notSupportedMessage);
        throw new NotSupportedException(notSupportedMessage);
    }

    public void SetEnvironmentVariables(IDictionary<string, string> variables)
    {
        ArgumentNullException.ThrowIfNull(variables);

        foreach (var (key, value) in variables)
        {
            System.Environment.SetEnvironmentVariable(key, value);
            LogEnvironmentInjectedVariable(key, value);
        }
    }
}