namespace Atc.Azure.IoT.Wpf.App;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow
{
    public MainWindow(
        IMainWindowViewModelBase viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModelBase;
        vm!.OnLoaded(this, e);
    }

    private void OnClosing(
        object? sender,
        CancelEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModelBase;
        vm!.OnClosing(this, e);
    }

    private void OnKeyDown(
        object sender,
        KeyEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModelBase;
        vm!.OnKeyDown(this, e);
    }

    private void OnKeyUp(
        object sender,
        KeyEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModelBase;
        vm!.OnKeyUp(this, e);
    }

    private void OnRibbonOnSelectedTabChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.AddedItems[0] is not RibbonTabItem tabItem)
        {
            return;
        }

        if (!Enum<ContextViewMode>.TryParse(tabItem.Tag?.ToString()!, out var contextViewMode))
        {
            return;
        }

        var vm = DataContext as MainWindowViewModel;

        if (vm!.ContextViewMode != contextViewMode)
        {
            vm!.ContextViewMode = contextViewMode;
        }
    }
}