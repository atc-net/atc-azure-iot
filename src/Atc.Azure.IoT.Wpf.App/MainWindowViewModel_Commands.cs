namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel
{
    public IRelayCommand OpenApplicationAboutCommand => new RelayCommand(OpenApplicationAboutCommandHandler);

    private void OpenApplicationAboutCommandHandler()
    {
        var aboutBoxDialog = new AboutBoxDialog
        {
            IconImage =
            {
                Source = App.DefaultIcon
            }
        };

        aboutBoxDialog.ShowDialog();
    }
}