namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel
{
    public IRelayCommand OpenApplicationAboutCommand => new RelayCommand(OpenApplicationAboutCommandHandler);

    public IRelayCommandAsync GetTenantsCommand => new RelayCommandAsync(
        GetTenantsCommandHandler,
        CanGetTenantsCommandHandler);


    public IRelayCommandAsync GetTestCommand1 => new RelayCommandAsync(GetTestCommand1Handler);

    private async Task GetTestCommand1Handler()
    {
        // TODO: Only 1 credential should be created and reused
        var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
        {
            TenantId = SelectedTenantId,
        });

        var armClient = new ArmClient(credential);

        var subscriptionCollection = armClient.GetSubscriptions();
        var asyncPageable = subscriptionCollection.GetAllAsync(CancellationToken.None);

        var subscriptions = new List<SubscriptionResource>();
        await foreach (var sub in asyncPageable.ConfigureAwait(false))
        {
            if (sub.Data.State != SubscriptionState.Enabled)
            {
                continue;
            }

            subscriptions.Add(sub);
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Subscriptions.AddRange(subscriptions);
        });
    }

    public IRelayCommandAsync GetTestCommand2 => new RelayCommandAsync(GetTestCommand2Handler);

    private async Task GetTestCommand2Handler()
    {
        var iotHubs = new List<IotHubDescriptionResource>();
        foreach (var subscriptionResource in Subscriptions)
        {
            var resourceGroupCollection = subscriptionResource.GetResourceGroups();

            var resourceGroupAsyncPageable = resourceGroupCollection.GetAllAsync();
            await foreach (var resourceGroup in resourceGroupAsyncPageable.ConfigureAwait(false))
            {
                var iotHubDescriptionCollection = resourceGroup.GetIotHubDescriptions();
                var iotHubCollectionAsyncPageable = iotHubDescriptionCollection.GetAllAsync(CancellationToken.None);
                await foreach (var iothub in iotHubCollectionAsyncPageable.ConfigureAwait(false))
                {
                    iotHubs.Add(iothub);
                }
            }
        }
    }

    public IRelayCommandAsync GetTestCommand3 => new RelayCommandAsync(GetTestCommand3Handler);

    private async Task GetTestCommand3Handler()
    {
        var deviceProvisioningServices = new List<DeviceProvisioningServiceResource>();
        foreach (var subscriptionResource in Subscriptions)
        {
            var resourceGroupCollection = subscriptionResource.GetResourceGroups();

            var resourceGroupAsyncPageable = resourceGroupCollection.GetAllAsync();
            await foreach (var resourceGroup in resourceGroupAsyncPageable.ConfigureAwait(false))
            {
                var deviceProvisioningServiceCollection = resourceGroup.GetDeviceProvisioningServices();
                var deviceProvisioningCollectionAsyncPageable = deviceProvisioningServiceCollection.GetAllAsync(CancellationToken.None);
                await foreach (var dps in deviceProvisioningCollectionAsyncPageable.ConfigureAwait(false))
                {
                    deviceProvisioningServices.Add(dps);
                }
            }
        }
    }

    private void OpenApplicationAboutCommandHandler()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var aboutBoxDialog = new AboutBoxDialog();
        aboutBoxDialog.IconImage.Source = App.DefaultIcon;
        aboutBoxDialog.ShowDialog();
    }

    private bool CanGetTenantsCommandHandler()
        => !isAuthorizedToAzure;

    private async Task GetTenantsCommandHandler()
    {
        if (!CanGetTenantsCommandHandler())
        {
            return;
        }

        AuthorizationText = "Authorizing...";

        try
        {
            Tenants.Clear();


            var data = await authService.GetTenants(CancellationToken.None).ConfigureAwait(false);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Tenants.AddRange(data);

                RaisePropertyChanged(nameof(Tenants));

                SelectedTenantId = Tenants.FirstOrDefault(x => x.IsCurrent)?.TenantId.ToString() ?? string.Empty;

                IsAuthorizedToAzure = true;
            });

        }
        catch (Exception ex)
        {
            AuthorizationText = "Authorization failed: " + ex.Message;
        }
    }
}