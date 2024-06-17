namespace Atc.Azure.IoT.Wpf.App.Models;

public sealed class IoTHubDeviceViewModel : ViewModelBase
{
    public string Id { get; set; } = string.Empty;

    public IotDeviceConnectionState ConnectionState { get; set; }

    public IotDeviceStatus Status { get; set; }

    public string StatusReason { get; set; } = string.Empty;

    public IotDeviceAuthenticationType AuthenticationType { get; set; }

    public bool IotEdgeDevice { get; set; }

    public IoTEdgeDeviceDetailsViewModel? DeviceDetails { get; set; }
}