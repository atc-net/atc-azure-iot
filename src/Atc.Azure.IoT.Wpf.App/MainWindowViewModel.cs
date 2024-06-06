namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModelBase
{
    private ContextViewMode contextViewMode;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainWindowViewModel()
    {
        // Dummy for XAML design view
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public MainWindowViewModel(
        StatusBarViewModel statusBarViewModel,
        AzureTenantSelectionViewModel azureTenantSelectionViewModel)
    {
        ArgumentNullException.ThrowIfNull(statusBarViewModel);
        ArgumentNullException.ThrowIfNull(azureTenantSelectionViewModel);

        ContextViewMode = ContextViewMode.TenantSelection;
        StatusBarViewModel = statusBarViewModel;
        AzureTenantSelectionViewModel = azureTenantSelectionViewModel;
    }

    public StatusBarViewModel StatusBarViewModel { get; set; }

    public AzureTenantSelectionViewModel AzureTenantSelectionViewModel { get; set; }

    public ContextViewMode ContextViewMode
    {
        get => contextViewMode;
        set
        {
            contextViewMode = value;
            RaisePropertyChanged();
        }
    }
}