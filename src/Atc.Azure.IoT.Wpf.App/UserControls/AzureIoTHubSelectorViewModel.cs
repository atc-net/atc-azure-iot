namespace Atc.Azure.IoT.Wpf.App.UserControls;

public class AzureIoTHubSelectorViewModel : ViewModelBase
{
    public ObservableCollectionEx<IoTHubSubscriptionViewModel> IotHubs { get; set; } = [];
}