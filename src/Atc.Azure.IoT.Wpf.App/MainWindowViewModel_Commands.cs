namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel
{
    public static IRelayCommand OpenApplicationAboutCommand => new RelayCommand(OpenApplicationAboutCommandHandler);

    private static void OpenApplicationAboutCommandHandler()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var aboutBoxDialog = new AboutBoxDialog();
        aboutBoxDialog.IconImage.Source = App.DefaultIcon;
        aboutBoxDialog.ShowDialog();
    }
}