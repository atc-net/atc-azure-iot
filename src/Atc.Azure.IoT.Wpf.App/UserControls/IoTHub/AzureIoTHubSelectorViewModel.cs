// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Azure.IoT.Wpf.App.UserControls.IoTHub;

public class AzureIoTHubSelectorViewModel : IoTViewModelBase, IDisposable
{
    private readonly AzureResourceStateService azureResourceStateService;
    private readonly AzureResourceManagerService azureResourceManagerService;
    private readonly CancellationTokenSource cancellationTokenSource;
    private IoTHubSubscriptionViewModel? selectedItem;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AzureIoTHubSelectorViewModel()
        : base(new ToastNotificationManager())
    {
        // Dummy for XAML design view
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AzureIoTHubSelectorViewModel(
        AzureResourceStateService azureResourceStateService,
        AzureResourceManagerService azureResourceManagerService,
        ToastNotificationManager toastNotificationManager)
        : base(toastNotificationManager)
    {
        this.azureResourceStateService = azureResourceStateService;
        this.azureResourceManagerService = azureResourceManagerService;
        cancellationTokenSource = new CancellationTokenSource();

        Messenger.Default.Register<SubscriptionsCollectionStateMessage>(this, OnSubscriptionsCollectionStateMessage);
    }

    public IoTHubSubscriptionViewModel? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            RaisePropertyChanged();
            Messenger.Default.Send(new SelectedIoTHubSubscriptionMessage(value));
        }
    }

    public ObservableCollectionEx<IoTHubSubscriptionViewModel> IotHubs { get; set; } = [];

    private void OnSubscriptionsCollectionStateMessage(
        SubscriptionsCollectionStateMessage obj)
    {
        switch (obj.CollectionActionType)
        {
            case CollectionActionType.Cleared:
                IotHubs.Clear();
                break;
            case CollectionActionType.Loaded:
                TaskHelper.FireAndForget(LoadIotHubs());
                break;
        }
    }

    private async Task LoadIotHubs()
    {
        SetBusyFlagAndNotify(true);
        IotHubs.Clear();

        var (succeeded, errorMessage) = await azureResourceManagerService
            .LoadIotHubServices(cancellationTokenSource.Token)
            .ConfigureAwait(false);

        if (!succeeded)
        {
            NotifyError("IoT Hub Services retrieval", errorMessage);
            return;
        }

        var result = azureResourceStateService.IoTHubServices
            .Select(iothubServiceState => new IoTHubSubscriptionViewModel
            {
                SubscriptionId = iothubServiceState.Subscription.Data.SubscriptionId,
                SubscriptionName = iothubServiceState.Subscription.Data.DisplayName,
                ResourceGroupName = iothubServiceState.ResourceGroup.Data.Name,
                IoTHubName = iothubServiceState.Resource.Data.Name,
            })
            .OrderBy(x => x.SubscriptionName)
            .ThenBy(x => x.ResourceGroupName)
            .ThenBy(x => x.IoTHubName)
            .ToList();

        await Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
        {
            IotHubs.AddRange(result);
        }).ConfigureAwait(false);

        SetBusyFlagAndNotify(false);
    }

    public void Dispose()
        => cancellationTokenSource.Dispose();
}