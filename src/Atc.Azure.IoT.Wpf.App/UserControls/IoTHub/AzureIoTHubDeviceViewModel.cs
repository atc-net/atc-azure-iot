namespace Atc.Azure.IoT.Wpf.App.UserControls.IoTHub;

public sealed class AzureIoTHubDeviceViewModel : IoTViewModelBase
{
    private readonly AzureResourceStateService azureResourceStateService;
    private readonly CancellationTokenSource cancellationTokenSource;
    private IoTHubSubscriptionViewModel? ioTHubSubscription;
    private IoTHubDeviceViewModel? iotDevice;

    public AzureIoTHubDeviceViewModel(
        ToastNotificationManager toastNotificationManager,
        AzureResourceStateService azureResourceStateService)
        : base(toastNotificationManager)
    {
        this.azureResourceStateService = azureResourceStateService;
        cancellationTokenSource = new CancellationTokenSource();

        Messenger.Default.Register<SelectedIoTHubDeviceMessage>(this, OnSelectedIoTHubDeviceMessage);
        Messenger.Default.Register<SelectedIoTHubSubscriptionMessage>(this, OnSelectedIoTHubSubscriptionMessage);
    }

    public IoTHubDeviceViewModel? IotDevice
    {
        get => iotDevice;
        set
        {
            iotDevice = value;
            RaisePropertyChanged();
        }
    }

    private void OnSelectedIoTHubDeviceMessage(
        SelectedIoTHubDeviceMessage obj)
    {
        IotDevice = obj.IoTHubDeviceViewModel;

        if (IotDevice is null)
        {
            return;
        }

        if (IotDevice.DeviceDetails is null)
        {
            TaskHelper.FireAndForget(LoadDeviceDetails());
        }
    }

    private void OnSelectedIoTHubSubscriptionMessage(
        SelectedIoTHubSubscriptionMessage obj)
    {
        ioTHubSubscription = obj.IoTHubSubscriptionViewModel;
        IotDevice = null;
    }

    private async Task LoadDeviceDetails()
    {
        SetBusyFlagAndNotify(true);

        var iotHubServiceState = azureResourceStateService.IoTHubServices.FirstOrDefault(x => x.Resource.Data.Name.Equals(ioTHubSubscription!.IoTHubName));

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


        var edgeAgentModuleTwin = await iotHubService.GetModuleTwin(
            IotDevice!.Id,
            EdgeAgentConstants.ModuleId,
            cancellationTokenSource.Token);

        if (edgeAgentModuleTwin is null)
        {
            return;
        }

        var edgeAgentReportedProperties = edgeAgentModuleTwin.GetReportedProperties<EdgeAgentReportedProperties>();
        IotDevice.DeviceDetails = IoTEdgeDeviceDetailsViewModelFactory.Create(edgeAgentReportedProperties);
    }
}