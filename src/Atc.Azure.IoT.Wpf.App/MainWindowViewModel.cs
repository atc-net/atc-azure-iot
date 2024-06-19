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
        DeviceProvisioningServiceViewModel azureDeviceProvisioningServiceViewModel,
        IotHubServiceViewModel azureIoTHubServiceViewModel)
    {
        ArgumentNullException.ThrowIfNull(statusBarViewModel);
        ArgumentNullException.ThrowIfNull(azureTenantSelectionViewModel);
        ArgumentNullException.ThrowIfNull(azureDeviceProvisioningServiceViewModel);
        ArgumentNullException.ThrowIfNull(azureIoTHubServiceViewModel);

        ContextViewMode = ContextViewMode.TenantSelection;
        StatusBar = statusBarViewModel;
        AzureTenantSelection = azureTenantSelectionViewModel;
        AzureDeviceProvisioningService = azureDeviceProvisioningServiceViewModel;
        AzureIoTHubService = azureIoTHubServiceViewModel;

        Messenger.Default.Register<IsBusyMessage>(this, OnBusyMessage);
    }

    public StatusBarViewModel StatusBar { get; set; }

    public AzureTenantSelectionViewModel AzureTenantSelection { get; set; }

    public DeviceProvisioningServiceViewModel AzureDeviceProvisioningService { get; set; }

    public IotHubServiceViewModel AzureIoTHubService { get; set; }

    public ContextViewMode ContextViewMode
    {
        get => contextViewMode;
        set
        {
            contextViewMode = value;
            RaisePropertyChanged();
        }
    }

    private void OnBusyMessage(
        IsBusyMessage obj)
    {
        if (AzureTenantSelection.IsBusy ||
            AzureDeviceProvisioningService.IsBusy ||
            AzureIoTHubService.IsBusy)
        {
            if (!IsBusy)
            {
                IsBusy = true;
            }

            return;
        }

        if (IsBusy)
        {
            IsBusy = false;
        }
    }
}