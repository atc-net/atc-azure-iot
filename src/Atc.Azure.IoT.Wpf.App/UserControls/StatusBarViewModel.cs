namespace Atc.Azure.IoT.Wpf.App.UserControls;

public sealed class StatusBarViewModel : ViewModelBase
{
    private string? authenticatedUser;

    public StatusBarViewModel()
    {
        Messenger.Default.Register<AuthenticatedUserMessage>(this, OnAuthenticatedUserMessage);
    }

    public string? AuthenticatedUser
    {
        get => authenticatedUser;
        set
        {
            authenticatedUser = value;
            RaisePropertyChanged();
        }
    }

    private void OnAuthenticatedUserMessage(
        AuthenticatedUserMessage obj)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            AuthenticatedUser = obj.UserName;
        });
    }
}