namespace Atc.Azure.IoT.Wpf.App.Controls;

public sealed class AzureTenantSelectionViewModel : ViewModelBase
{
    private readonly AzureAuthService azureAuthService;
    private readonly AzureResourceManagerService azureResourceManagerService;
    private readonly ToastNotificationManager toastNotificationManager;
    private readonly CancellationTokenSource cancellationTokenSource;

    private bool isAuthorizedToAzure;
    private bool isAuthorizedSelectedTenant;
    private string? selectedTenantId;
    private Dictionary<string, string> tenants = [];

    public IRelayCommandAsync SignInToAzureCommand => new RelayCommandAsync(
        SignInToAzureCommandHandler,
        CanSignInToAzureCommandHandler);

    public AzureTenantSelectionViewModel(
        AzureAuthService azureAuthService,
        AzureResourceManagerService azureResourceManagerService,
        ToastNotificationManager toastNotificationManager)
    {
        this.azureAuthService = azureAuthService;
        this.azureResourceManagerService = azureResourceManagerService;
        this.toastNotificationManager = toastNotificationManager;
        cancellationTokenSource = new CancellationTokenSource();
    }

    public Dictionary<string, string> Tenants
    {
        get => tenants;
        set
        {
            tenants = value;
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

    public string? SelectedTenantId
    {
        get => selectedTenantId;
        set
        {
            selectedTenantId = value;
            RaisePropertyChanged();
        }
    }

    private bool CanSignInToAzureCommandHandler()
        => !isAuthorizedToAzure;

    private async Task SignInToAzureCommandHandler()
    {
        if (!CanSignInToAzureCommandHandler())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var (succeeded, errorMessage) = await azureAuthService
                .SignIn(cancellationTokenSource.Token)
                .ConfigureAwait(false);

            if (!succeeded ||
                string.IsNullOrEmpty(azureAuthService.AuthenticationRecord?.Username))
            {
                throw new Exception(errorMessage);
            }

            Messenger.Default.Send(new AuthenticatedUserMessage(azureAuthService.AuthenticationRecord.Username));

            var (tenantSucceeded, tenantResources, tenantErrorMessage) = await azureResourceManagerService
                .GetTenants(azureAuthService.Credential!, cancellationTokenSource.Token)
                .ConfigureAwait(false);

            if (!tenantSucceeded ||
                tenantResources is null)
            {
                throw new Exception(tenantErrorMessage);
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Tenants = tenantResources
                    .OrderBy(x => x.Data.DisplayName)
                    .ToDictionary(x => x.Data.TenantId!.ToString()!, x => x.Data.DisplayName);

                SelectedTenantId = azureAuthService.AuthenticationRecord.TenantId;
                IsAuthorizedToAzure = true;
            });
        }
        catch (Exception ex)
        {

            var content = new ToastNotificationContent(
                ToastNotificationType.Error,
                "Sign in error",
                ex.GetLastInnerMessage());

            toastNotificationManager.Show(false, areaName: "ToastNotificationArea", content: content);
        }
        finally
        {
            IsBusy = false;
        }
    }
}