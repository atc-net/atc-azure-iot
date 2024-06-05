namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModelBase
{
    private readonly AuthService authService;
    private string authorizationText;
    private bool isAuthorizedToAzure;
    private bool isAuthorizedSelectedTenant;
    private string selectedTenantId;

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

        authService = new AuthService();
        StatusBarViewModel = statusBarViewModel;
        AuthorizationText = "N/A";

        if (IsInDesignMode)
        {
            Tenants.Add(new TenantViewModel { DisplayName = "heh", TenantId = Guid.NewGuid(), IsCurrent = true });
            Tenants.Add(new TenantViewModel { DisplayName = "heh2", TenantId = Guid.NewGuid() });
            Tenants.Add(new TenantViewModel { DisplayName = "heh3", TenantId = Guid.NewGuid() });
        }
    }

    public StatusBarViewModel StatusBarViewModel { get; set; }

    public ObservableCollectionEx<TenantViewModel> Tenants { get; set; } = [];

    public string AuthorizationText
    {
        get => authorizationText;
        set
        {
            authorizationText = value;
            RaisePropertyChanged();
        }
    }

    public bool IsAuthorizedToAzure
    {
        get => isAuthorizedToAzure;
        set
        {
            isAuthorizedToAzure = value;
            RaisePropertyChanged();
        }
    }

    public string SelectedTenantId
    {
        get => selectedTenantId;
        set
        {
            selectedTenantId = value;
            RaisePropertyChanged();
        }
    }
}