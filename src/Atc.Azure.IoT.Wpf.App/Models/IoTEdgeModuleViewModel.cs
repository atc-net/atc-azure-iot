namespace Atc.Azure.IoT.Wpf.App.Models;

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

    public override string ToString()
        => $"{nameof(ModuleId)}: {ModuleId}, {nameof(UpstreamProtocol)}: {UpstreamProtocol}, {nameof(StartupOrder)}: {StartupOrder}, {nameof(ContainerImage)}: {ContainerImage}, {nameof(State)}: ({State}), {nameof(LastExitTimeUtc)}: {LastExitTimeUtc}, {nameof(LastStartTimeUtc)}: {LastStartTimeUtc}, {nameof(LastRestartTimeUtc)}: {LastRestartTimeUtc}, {nameof(RestartCount)}: {RestartCount}, {nameof(ExitCode)}: {ExitCode}, {nameof(StatusDescription)}: {StatusDescription}";
}
