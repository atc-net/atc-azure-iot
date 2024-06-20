namespace Atc.Azure.IoT.Wpf.App.ViewModels;

public sealed class AzureTenantSelectionViewModel : NotifyViewModelBase, IDisposable
{
    private readonly AzureAuthService azureAuthService;
    private readonly AzureResourceManagerService azureResourceManagerService;
    private readonly CancellationTokenSource cancellationTokenSource;

    private bool isAuthorizedToAzure;
    private Dictionary<string, string> tenants = [];
    private KeyValuePair<string, string>? selectedTenantId;

    public IRelayCommandAsync SignInToAzureCommand => new RelayCommandAsync(
        SignInToAzureCommandHandler,
        CanSignInToAzureCommandHandler);

    public IRelayCommandAsync<KeyValuePair<string, string>> OnTenantSelectionChangedCommand
        => new RelayCommandAsync<KeyValuePair<string, string>>(OnTenantSelectionChangedCommandHandler);

    public AzureTenantSelectionViewModel(
        AzureAuthService azureAuthService,
        AzureResourceManagerService azureResourceManagerService,
        ToastNotificationManager toastNotificationManager)
        : base(toastNotificationManager)
    {
        this.azureAuthService = azureAuthService;
        this.azureResourceManagerService = azureResourceManagerService;
        cancellationTokenSource = new CancellationTokenSource();
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

    public Dictionary<string, string> Tenants
    {
        get => tenants;
        set
        {
            tenants = value;
            RaisePropertyChanged();
        }
    }

    public KeyValuePair<string, string>? SelectedTenant
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

        SetBusyFlagAndNotify(this, true);

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

            await Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
            {
                Tenants = tenantResources
                    .OrderBy(x => x.Data.DisplayName, StringComparer.Ordinal)
                    .ToDictionary(x => x.Data.TenantId!.ToString()!, x => x.Data.DisplayName);

                SelectedTenant = Tenants.First(x => x.Key == azureAuthService.AuthenticationRecord.TenantId);
                IsAuthorizedToAzure = true;
            }).ConfigureAwait(false);

            Messenger.Default.Send(new AuthenticatedUserMessage(
                UserName: azureAuthService.AuthenticationRecord.Username,
                TenantName: Tenants.Single(x => x.Key.Equals(azureAuthService.AuthenticationRecord.TenantId, StringComparison.Ordinal)).Value));

            await ReloadSubscriptions().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            NotifyError("Sign in error", ex.GetLastInnerMessage());
        }
        finally
        {
            SetBusyFlagAndNotify(this, false);
        }
    }

    private Task OnTenantSelectionChangedCommandHandler(
        KeyValuePair<string, string> arg)
    {
        if (string.IsNullOrEmpty(arg.Key))
        {
            return Task.CompletedTask;
        }

        var tenantId = Guid.Parse(arg.Key);
        return ChangeTenant(tenantId);
    }

    private async Task ChangeTenant(
        Guid tenantId)
    {
        SetBusyFlagAndNotify(this, true);

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
                TenantName: Tenants.Single(x => x.Key.Equals(azureAuthService.AuthenticationRecord.TenantId, StringComparison.Ordinal)).Value));

            await ReloadSubscriptions().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            NotifyError("Sign in error", ex.GetLastInnerMessage());
        }
        finally
        {
            SetBusyFlagAndNotify(this, false);
        }
    }

    private async Task ReloadSubscriptions()
    {
        Messenger.Default.Send(new SubscriptionsCollectionStateMessage(CollectionActionType.Cleared));

        var (succeeded, errorMessage) = await azureResourceManagerService
            .LoadSubscriptions(
                azureAuthService.Credential!,
                cancellationTokenSource.Token)
            .ConfigureAwait(false);

        if (!succeeded)
        {
            NotifyError("Subscription retrieval", errorMessage);
            return;
        }

        Messenger.Default.Send(new SubscriptionsCollectionStateMessage(CollectionActionType.Loaded));
    }

    public void Dispose()
        => cancellationTokenSource.Dispose();
}