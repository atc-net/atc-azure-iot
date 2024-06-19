namespace Atc.Azure.IoT.Wpf.App.UserControls.IotHub;

public class IoTEdgeModuleViewModel : ViewModelBase
{
    public string ModuleId { get; set; } = string.Empty;

    public string? UpstreamProtocol { get; set; }

    public int StartupOrder { get; set; }

    public string ContainerImage { get; set; } = string.Empty;

    /// <summary>
    /// Represents the state of an iot edge module.
    /// </summary>
    public IotEdgeModuleState State { get; set; }

    public DateTimeOffset? LastExitTimeUtc { get; set; }

    public DateTimeOffset? LastStartTimeUtc { get; set; }

    public DateTimeOffset? LastRestartTimeUtc { get; set; }

    public int RestartCount { get; set; }

    public int ExitCode { get; set; }

    public string StatusDescription { get; set; } = string.Empty;
}