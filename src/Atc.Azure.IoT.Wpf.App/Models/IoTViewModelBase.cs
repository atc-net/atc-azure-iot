namespace Atc.Azure.IoT.Wpf.App.Models;

public abstract class IoTViewModelBase : ViewModelBase
{
    private readonly ToastNotificationManager toastNotificationManager;

    protected IoTViewModelBase(
        ToastNotificationManager toastNotificationManager)
    {
        this.toastNotificationManager = toastNotificationManager;
    }

    public void SetBusyFlagAndNotify(
        bool value)
    {
        IsBusy = value;
        Messenger.Default.Send(new IsBusyMessage(IsBusy));
    }

    public void NotifyError(
        string title,
        string? errorMessage)
    {
        var content = new ToastNotificationContent(
            ToastNotificationType.Error,
            title,
            errorMessage ?? "N/A");

        toastNotificationManager.Show(false, areaName: "ToastNotificationArea", content: content);
    }
}