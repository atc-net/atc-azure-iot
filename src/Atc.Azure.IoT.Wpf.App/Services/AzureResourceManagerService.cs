namespace Atc.Azure.IoT.Wpf.App.Services;

public sealed class AzureResourceManagerService // TODO: Interface
{
    private ArmClient? armClient;

    public async Task<(bool Succeeded, List<TenantResource>? Data, string? ErrorMessage)> GetTenants(
        TokenCredential credential,
        CancellationToken cancellationToken)
    {
        armClient ??= new ArmClient(credential);

        var result = new List<TenantResource>();

        try
        {
            await foreach (var tenant in armClient.GetTenants().GetAllAsync(cancellationToken).ConfigureAwait(false))
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

    public async Task<(bool Succeeded, List<SubscriptionResource>? Data, string? ErrorMessage)> GetSubscriptionsByTenantId(
        TokenCredential credential,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        armClient ??= new ArmClient(credential);

        var result = new List<SubscriptionResource>();

        try
        {
            await foreach (var sub in armClient.GetSubscriptions().GetAllAsync(cancellationToken).ConfigureAwait(false))
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
            return (false, null, ex.GetLastInnerMessage());
        }

        return (true, result, null);
    }
}