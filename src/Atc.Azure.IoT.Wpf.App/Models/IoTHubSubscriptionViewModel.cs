namespace Atc.Azure.IoT.Wpf.App.Models;

public class IoTHubSubscriptionViewModel
{
    public string SubscriptionId { get; set; } = string.Empty;

    public string SubscriptionName { get; set; } = string.Empty;

    public string ResourceGroupName { get; set; } = string.Empty;

    public string IoTHubName { get; set; } = string.Empty;

    // TODO:
    // Data.Location
    // Data.Sku (Capacity, Name)

    // Filter on ProvisioningState == Succeeded
}