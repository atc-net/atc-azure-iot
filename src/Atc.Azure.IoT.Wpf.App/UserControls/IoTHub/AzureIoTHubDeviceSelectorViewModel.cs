namespace Atc.Azure.IoT.Wpf.App.UserControls.IoTHub;

public class AzureIoTHubDeviceSelectorViewModel : IoTViewModelBase
{
    private readonly AzureResourceStateService azureResourceStateService;
    private IoTHubDeviceViewModel? selectedDevice;

    public AzureIoTHubDeviceSelectorViewModel(
        ToastNotificationManager toastNotificationManager,
        AzureResourceStateService azureResourceStateService)
        : base(toastNotificationManager)
    {
        this.azureResourceStateService = azureResourceStateService;

        Messenger.Default.Register<SubscriptionsCollectionStateMessage>(this, OnSubscriptionsCollectionStateMessage);
        Messenger.Default.Register<SelectedIoTHubSubscriptionMessage>(this, OnSelectedIoTHubSubscriptionMessage);
    }

    private void OnSubscriptionsCollectionStateMessage(
        SubscriptionsCollectionStateMessage obj)
    {
        if (obj.CollectionActionType != CollectionActionType.Cleared)
        {
            return;
        }

        SelectedDevice = null;
        Devices.Clear();
    }

    private void OnSelectedIoTHubSubscriptionMessage(
        SelectedIoTHubSubscriptionMessage obj)
    {
        if (obj.IoTHubSubscriptionViewModel is null)
        {
            return;
        }

        TaskHelper.FireAndForget(
            LoadDevices(
                obj.IoTHubSubscriptionViewModel.IoTHubName));
    }

    public IoTHubDeviceViewModel? SelectedDevice
    {
        get => selectedDevice;
        set
        {
            selectedDevice = value;
            RaisePropertyChanged();
            Messenger.Default.Send(new SelectedIoTHubDeviceMessage(value));
        }
    }

    public ObservableCollectionEx<IoTHubDeviceViewModel> Devices { get; set; } = [];

    private async Task LoadDevices(
        string iotHubName)
    {
        SetBusyFlagAndNotify(true);
        Devices.Clear();

        var iotHubServiceState = azureResourceStateService.IoTHubServices.FirstOrDefault(x => x.Resource.Data.Name.Equals(iotHubName));

        if (iotHubServiceState is null ||
            string.IsNullOrEmpty(iotHubServiceState.ConnectionString))
        {
            SetBusyFlagAndNotify(false);
            return;
        }

        var iotHubService = new IoTHubService(
            NullLoggerFactory.Instance,
            new IoTHubModuleService(NullLoggerFactory.Instance, iotHubServiceState.ConnectionString),
            iotHubServiceState.ConnectionString!);

        var devices = await iotHubService.GetDevices().ConfigureAwait(false);
        if (!devices.Any())
        {
            SetBusyFlagAndNotify(false);
            return;
        }

        await Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
        {
            Devices.AddRange(devices.Select(x => new IoTHubDeviceViewModel
            {
                Id = x.DeviceId,
                ConnectionState = x.ConnectionState,
                Status = x.Status,
                StatusReason = x.StatusReason,
                AuthenticationType = x.AuthenticationType,
                IotEdgeDevice = x.Capabilities.IotEdge,
            }));
        }).ConfigureAwait(false);

        SetBusyFlagAndNotify(false);
    }
}