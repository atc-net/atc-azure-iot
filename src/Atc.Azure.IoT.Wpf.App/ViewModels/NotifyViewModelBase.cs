namespace Atc.Azure.IoT.Wpf.App.ViewModels;

public abstract class NotifyViewModelBase : ViewModelBase
{
    private readonly ToastNotificationManager toastNotificationManager;

    protected NotifyViewModelBase(
        ToastNotificationManager toastNotificationManager)
    {
        this.toastNotificationManager = toastNotificationManager;
    }

    public void SetBusyFlagAndNotify(
        object sender,
        bool value)
    {
        IsBusy = value;
        Messenger.Default.Send(new IsBusyMessage(sender, IsBusy));
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