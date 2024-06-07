namespace Atc.Azure.IoT.Wpf.App.Services;

public sealed class AzureResourceManagerService // TODO: Interface  
{
    private readonly AzureResourceStateService azureResourceStateService;

    public AzureResourceManagerService(
        AzureResourceStateService azureResourceStateService)
    {
        this.azureResourceStateService = azureResourceStateService;
    }

    public async Task<(bool Succeeded, List<TenantResource>? Data, string? ErrorMessage)> GetTenants(
        TokenCredential credential,
        CancellationToken cancellationToken)
    {
        var armClient = new ArmClient(credential);

        var result = new List<TenantResource>();

        try
        {
            await foreach (var tenant in armClient
                               .GetTenants()
                               .GetAllAsync(cancellationToken)
                               .ConfigureAwait(false))
            {
                if (tenant.Data.TenantId is null)
                {
                    continue;
                }

                result.Add(tenant);
            }
        }
        catch (Exception ex)
        {
            return (false, null, ex.GetLastInnerMessage());
        }

        return (true, result, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> LoadSubscriptions(
        TokenCredential credential,
        CancellationToken cancellationToken)
    {
        var armClient = new ArmClient(credential);

        azureResourceStateService.Subscriptions.Clear();

        var result = new List<SubscriptionResource>();

        try
        {
            await foreach (var sub in armClient
                               .GetSubscriptions()
                               .GetAllAsync(cancellationToken)
                               .ConfigureAwait(false))
            {
                if (sub.Data.State != SubscriptionState.Enabled)
                {
                    continue;
                }

                result.Add(sub);
            }
        }
        catch (Exception ex)
        {
            return (false, ex.GetLastInnerMessage());
        }

        azureResourceStateService.Subscriptions.AddRange(result);

        return (true, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> LoadIotHubServices(
        CancellationToken cancellationToken)
    {
        azureResourceStateService.IoTHubServices.Clear();

        var result = new List<IotHubServiceState>();

        try
        {
            foreach (var subscription in azureResourceStateService.Subscriptions)
            {
                await foreach (var resourceGroup in subscription
                                   .GetResourceGroups()
                                   .GetAllAsync(cancellationToken: cancellationToken)
                                   .ConfigureAwait(false))
                {
                    await foreach (var iothub in resourceGroup
                                       .GetIotHubDescriptions()
                                       .GetAllAsync(cancellationToken)
                                       .ConfigureAwait(false))
                    {
                        result.Add(new IotHubServiceState(subscription, resourceGroup, iothub));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return (false, ex.GetLastInnerMessage());
        }

        azureResourceStateService.IoTHubServices.AddRange(result);

        return (true, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> LoadDeviceProvisioningServices(
        CancellationToken cancellationToken)
    {
        azureResourceStateService.DeviceProvisioningServices.Clear();

        var result = new List<DeviceProvisioningServiceState>();

        try
        {
            foreach (var subscription in azureResourceStateService.Subscriptions)
            {
                await foreach (var resourceGroup in subscription
                                   .GetResourceGroups()
                                   .GetAllAsync(cancellationToken: cancellationToken)
                                   .ConfigureAwait(false))
                {
                    await foreach (var dps in resourceGroup
                                       .GetDeviceProvisioningServices()
                                       .GetAllAsync(cancellationToken)
                                       .ConfigureAwait(false))
                    {
                        result.Add(new DeviceProvisioningServiceState(subscription, resourceGroup, dps));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return (false, ex.GetLastInnerMessage());
        }

        azureResourceStateService.DeviceProvisioningServices.AddRange(result);

        return (true, null);
    }
}