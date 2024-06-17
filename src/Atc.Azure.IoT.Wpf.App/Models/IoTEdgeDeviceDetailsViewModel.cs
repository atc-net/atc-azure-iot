namespace Atc.Azure.IoT.Wpf.App.Models;

public class IoTEdgeDeviceDetailsViewModel : ViewModelBase
{
    public int RuntimeStatusCode { get; set; }

    public string RuntimeStatusDescription { get; set; } = string.Empty;

    public string OperatingSystem { get; set; } = string.Empty;

    public string OperatingSystemArchitecture { get; set; } = string.Empty;

    public List<IoTEdgeModuleViewModel> SystemModules { get; set; } = [];

    public List<IoTEdgeModuleViewModel> CustomModules { get; set; } = [];

    public override string ToString()
        => $"{nameof(RuntimeStatusCode)}: {RuntimeStatusCode}, {nameof(RuntimeStatusDescription)}: {RuntimeStatusDescription}, {nameof(OperatingSystem)}: {OperatingSystem}, {nameof(OperatingSystemArchitecture)}: {OperatingSystemArchitecture}";
}