namespace Atc.Azure.IoT.Wpf.App.UserControls;

public sealed class AzureTenantSelectionViewModel : ViewModelBase, IDisposable
{
    private readonly AzureAuthService azureAuthService;
    private readonly AzureResourceManagerService azureResourceManagerService;
    private readonly ToastNotificationManager toastNotificationManager;
    private readonly CancellationTokenSource cancellationTokenSource;

    private bool isAuthorizedToAzure;
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

    public void OnTenantSelectedChangedHandler(
        object? sender,
        ValueChangedEventArgs<string?> e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.NewValue! == SelectedTenantId)
        {
            return;
        }

        var tenantId = Guid.Parse(e.NewValue!);

        TaskHelper.FireAndForget(ChangeTenant(tenantId));
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
                    .OrderBy(x => x.Data.DisplayName, StringComparer.Ordinal)
                    .ToDictionary(x => x.Data.TenantId!.ToString()!, x => x.Data.DisplayName);

                SelectedTenantId = azureAuthService.AuthenticationRecord.TenantId;
                IsAuthorizedToAzure = true;
            });

            Messenger.Default.Send(new AuthenticatedUserMessage(
                UserName: azureAuthService.AuthenticationRecord.Username,
                TenantName: Tenants.Single(x => x.Key.Equals(azureAuthService.AuthenticationRecord.TenantId)).Value));
        }
        catch (Exception ex)
        {
            NotifySignInError(ex.GetLastInnerMessage());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ChangeTenant(
        Guid tenantId)
    {
        IsBusy = true;

        try
        {
            var (succeeded, errorMessage) = await azureAuthService
                .SignInToTenant(tenantId, cancellationTokenSource.Token)
                .ConfigureAwait(false);

            if (!succeeded)
            {
                throw new Exception(errorMessage);
            }
            
            Messenger.Default.Send(new AuthenticatedUserMessage(
                UserName: azureAuthService.AuthenticationRecord!.Username,
                TenantName: Tenants.Single(x => x.Key.Equals(azureAuthService.AuthenticationRecord.TenantId)).Value));
        }
        catch (Exception ex)
        {
            NotifySignInError(ex.GetLastInnerMessage());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void NotifySignInError(
        string errorMessage)
    {
        var content = new ToastNotificationContent(
            ToastNotificationType.Error,
            "Sign in error",
            errorMessage);

        toastNotificationManager.Show(false, areaName: "ToastNotificationArea", content: content);
    }

    public void Dispose()
        => cancellationTokenSource.Dispose();
}