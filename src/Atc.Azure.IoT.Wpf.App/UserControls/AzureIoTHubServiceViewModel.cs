namespace Atc.Azure.IoT.Wpf.App.UserControls;

public class AzureIoTHubServiceViewModel : ViewModelBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AzureIoTHubServiceViewModel()
    {
        // Dummy for XAML design view
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AzureIoTHubServiceViewModel(
        IoTHubSubscriptionViewModel ioTHubSubscriptionViewModel)
    {
        ArgumentNullException.ThrowIfNull(ioTHubSubscriptionViewModel);

        IoTHubSubscriptionViewModel = ioTHubSubscriptionViewModel;
    }

    public IoTHubSubscriptionViewModel IoTHubSubscriptionViewModel { get; set; }
}