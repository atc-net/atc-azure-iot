// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Azure.IoT.Wpf.App.UserControls.IotHub;

public class IotHubServiceViewModel : NotifyViewModelBase, IDisposable
{
    private readonly AzureResourceStateService azureResourceStateService;
    private readonly AzureResourceManagerService azureResourceManagerService;
    private readonly CancellationTokenSource cancellationTokenSource;

    private bool isAuthorizedToAzure;
    private IotHubServiceState? selectedIotHub;
    private IotDevice? selectedIotDevice;
    private IotDeviceDetailsViewModel? selectedIotDeviceDetails;

    public IRelayCommandAsync<IotHubServiceState> OnIotHubSelectionChangedCommand
        => new RelayCommandAsync<IotHubServiceState>(OnIotHubSelectionChangedCommandHandler);

    public IRelayCommandAsync<IotDevice> OnIotDeviceSelectionChangedCommand
        => new RelayCommandAsync<IotDevice>(OnIotDeviceSelectionChangedCommandHandler);

    public IotHubServiceViewModel(
        AzureResourceStateService azureResourceStateService,
        AzureResourceManagerService azureResourceManagerService,
        ToastNotificationManager toastNotificationManager)
        : base(toastNotificationManager)
    {
        this.azureResourceStateService = azureResourceStateService;
        this.azureResourceManagerService = azureResourceManagerService;
        cancellationTokenSource = new CancellationTokenSource();

        Messenger.Default.Register<AuthenticatedUserMessage>(this, OnAuthenticatedUserMessage);
        Messenger.Default.Register<SubscriptionsCollectionStateMessage>(this, OnSubscriptionsCollectionStateMessage);
    }

    public bool IsAuthorizedToAzure
    {
        get => isAuthorizedToAzure;
        set
        {
            isAuthorizedToAzure = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollectionEx<IotHubServiceState> IotHubs { get; set; } = new();

    public IotHubServiceState? SelectedIotHub
    {
        get => selectedIotHub;
        set
        {
            selectedIotHub = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollectionEx<IotDevice> IotDevices { get; set; } = new();

    public IotDevice? SelectedIotDevice
    {
        get => selectedIotDevice;
        set
        {
            selectedIotDevice = value;
            RaisePropertyChanged();
        }
    }

    public IotDeviceDetailsViewModel? SelectedIotDeviceDetails
    {
        get => selectedIotDeviceDetails;
        set
        {
            selectedIotDeviceDetails = value;
            RaisePropertyChanged();
        }
    }

    protected virtual void Dispose(
        bool disposing)
    {
        if (disposing)
        {
            cancellationTokenSource.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void OnAuthenticatedUserMessage(
        AuthenticatedUserMessage obj)
        => IsAuthorizedToAzure = !string.IsNullOrEmpty(obj.UserName);

    private void OnSubscriptionsCollectionStateMessage(
        SubscriptionsCollectionStateMessage obj)
    {
        switch (obj.CollectionActionType)
        {
            case CollectionActionType.Cleared:
                ClearAll();
                break;
            case CollectionActionType.Loaded:
                TaskHelper.FireAndForget(LoadIotHubs());
                break;
        }
    }

    private Task OnIotHubSelectionChangedCommandHandler(
        IotHubServiceState arg)
        => LoadIotDevices(arg);

    private Task OnIotDeviceSelectionChangedCommandHandler(
        IotDevice arg)
        => LoadIotDeviceDetails(arg);

    private void ClearAll()
    {
        SelectedIotDevice = null;
        SelectedIotDeviceDetails = null;
        IotDevices.Clear();

        SelectedIotHub = null;
        IotHubs.Clear();
    }

    private async Task LoadIotHubs()
    {
        await Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
        {
            IsBusy = true;
            ClearAll();
        }).ConfigureAwait(false);

        var (succeeded, errorMessage) = await azureResourceManagerService
            .LoadIotHubServices(cancellationTokenSource.Token)
            .ConfigureAwait(false);

        if (!succeeded)
        {
            NotifyError("IoT Hub Services retrieval", errorMessage);
            return;
        }

        if (azureResourceStateService.IoTHubServices.Count > 0)
        {
            await Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
            {
                IotHubs.AddRange(
                    azureResourceStateService
                        .IoTHubServices
                        .OrderBy(x => x.ResourceGroup.Data.Name)
                        .ThenBy(x => x.Resource.Data.Name));
            }).ConfigureAwait(false);
        }

        IsBusy = false;
    }

    private async Task LoadIotDevices(
        IotHubServiceState iotHubServiceState)
    {
        if (iotHubServiceState is null ||
            string.IsNullOrEmpty(iotHubServiceState.ConnectionString))
        {
            return;
        }

        IsBusy = true;

        SelectedIotDevice = null;
        SelectedIotDeviceDetails = null;
        IotDevices.Clear();

        using var iotHubModuleService = new IoTHubModuleService(
            NullLoggerFactory.Instance,
            iotHubServiceState.ConnectionString);

        using var iotHubService = new IoTHubService(
            NullLoggerFactory.Instance,
            iotHubModuleService,
            iotHubServiceState.ConnectionString!);

        var devices = await iotHubService
            .GetDevices()
            .ConfigureAwait(false);

        if (devices.Count > 0)
        {
            await Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
            {
                IotDevices.AddRange(
                    devices
                        .OrderBy(x => x.DeviceId));
            }).ConfigureAwait(false);
        }

        IsBusy = false;
    }

    private async Task LoadIotDeviceDetails(
        IotDevice iotDevice)
    {
        if (iotDevice is null ||
            SelectedIotHub is null)
        {
            return;
        }

        IsBusy = true;

        SelectedIotDeviceDetails = null;

        var iotHubServiceState = azureResourceStateService.IoTHubServices.Find(x => x.Resource.Data.Id.ToString().Equals(SelectedIotHub.Resource.Data.Id.ToString(), StringComparison.Ordinal));

        if (iotHubServiceState is not null &&
            !string.IsNullOrEmpty(iotHubServiceState.ConnectionString))
        {
            using var iotHubModuleService = new IoTHubModuleService(
                NullLoggerFactory.Instance,
                iotHubServiceState.ConnectionString);

            using var iotHubService = new IoTHubService(
                NullLoggerFactory.Instance,
                iotHubModuleService,
                iotHubServiceState.ConnectionString!);

            var moduleTwin = await iotHubService
                .GetModuleTwin(
                    iotDevice.DeviceId,
                    EdgeAgentConstants.ModuleId,
                    cancellationTokenSource.Token)
                .ConfigureAwait(false);

            if (moduleTwin is not null)
            {
                var edgeAgentReportedProperties = moduleTwin.GetReportedProperties<EdgeAgentReportedProperties>();
                SelectedIotDeviceDetails = IoTEdgeDeviceDetailsViewModelFactory.Create(edgeAgentReportedProperties);
            }
        }

        IsBusy = false;
    }
}