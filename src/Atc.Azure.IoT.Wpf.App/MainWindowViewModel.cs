namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModelBase
{
    private ContextViewMode contextViewMode;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainWindowViewModel()
    {
        // Dummy for XAML design view
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public MainWindowViewModel(
        StatusBarViewModel statusBarViewModel,
        AzureTenantSelectionViewModel azureTenantSelectionViewModel,
        AzureDeviceProvisioningServiceViewModel azureDeviceProvisioningServiceViewModel,
        AzureIoTHubServiceViewModel azureIoTHubServiceViewModel)
    {
        ArgumentNullException.ThrowIfNull(statusBarViewModel);
        ArgumentNullException.ThrowIfNull(azureTenantSelectionViewModel);
        ArgumentNullException.ThrowIfNull(azureDeviceProvisioningServiceViewModel);
        ArgumentNullException.ThrowIfNull(azureIoTHubServiceViewModel);

        ContextViewMode = ContextViewMode.TenantSelection;
        StatusBarViewModel = statusBarViewModel;
        AzureTenantSelectionViewModel = azureTenantSelectionViewModel;
        AzureDeviceProvisioningServiceViewModel = azureDeviceProvisioningServiceViewModel;
        AzureIoTHubServiceViewModel = azureIoTHubServiceViewModel;
    }

    public StatusBarViewModel StatusBarViewModel { get; set; }

    public AzureTenantSelectionViewModel AzureTenantSelectionViewModel { get; set; }

    public AzureDeviceProvisioningServiceViewModel AzureDeviceProvisioningServiceViewModel { get; set; }

    public AzureIoTHubServiceViewModel AzureIoTHubServiceViewModel { get; set; }

    public ContextViewMode ContextViewMode
    {
        get => contextViewMode;
        set
        {
            contextViewMode = value;
            RaisePropertyChanged();
        }
    }
}