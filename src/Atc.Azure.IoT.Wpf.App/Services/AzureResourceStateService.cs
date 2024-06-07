namespace Atc.Azure.IoT.Wpf.App.Services;

public sealed class AzureResourceStateService // TODO: Interface
{
    public List<SubscriptionResource> Subscriptions { get; private set; } = [];

    public List<IotHubServiceState> IoTHubServices { get; private set; } = [];

    public List<DeviceProvisioningServiceState> DeviceProvisioningServices { get; private set; } = [];
}