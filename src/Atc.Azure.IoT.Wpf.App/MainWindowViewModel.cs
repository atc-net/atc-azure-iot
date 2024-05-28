namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModelBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainWindowViewModel()
    {
        // Dummy for XAML design view
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public MainWindowViewModel(
        StatusBarViewModel statusBarViewModel)
    {
        ArgumentNullException.ThrowIfNull(statusBarViewModel);

        StatusBarViewModel = statusBarViewModel;
    }

    public StatusBarViewModel StatusBarViewModel { get; set; }
}