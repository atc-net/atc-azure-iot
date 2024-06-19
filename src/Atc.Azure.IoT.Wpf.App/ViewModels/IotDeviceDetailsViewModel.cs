namespace Atc.Azure.IoT.Wpf.App.ViewModels;

public class IotDeviceDetailsViewModel : ViewModelBase
{
    public string OperatingSystem { get; set; } = string.Empty;

    public string OperatingSystemArchitecture { get; set; } = string.Empty;

    public int RuntimeStatusCode { get; set; }

    public string RuntimeStatusDescription { get; set; } = string.Empty;

    public IList<IoTEdgeModuleViewModel> SystemModules { get; set; } = [];

    public IList<IoTEdgeModuleViewModel> CustomModules { get; set; } = [];
}