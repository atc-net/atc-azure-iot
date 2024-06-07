namespace Atc.Azure.IoT.Wpf.App.UserControls.IoTHub;

public class AzureIoTHubServiceViewModel : ViewModelBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AzureIoTHubServiceViewModel()
    {
        // Dummy for XAML design view
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AzureIoTHubServiceViewModel(
        AzureIoTHubSelectorViewModel azureIoTHubSelectorViewModel)
    {
        ArgumentNullException.ThrowIfNull(azureIoTHubSelectorViewModel);

        AzureIoTHubSelectorViewModel = azureIoTHubSelectorViewModel;
    }

    public AzureIoTHubSelectorViewModel AzureIoTHubSelectorViewModel { get; set; }
}