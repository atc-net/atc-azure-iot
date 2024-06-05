namespace Atc.Azure.IoT.Wpf.App.Authentication;

public sealed class AuthService
{
    private static readonly string[] DefaultScopes = ["openid", "offline_access"];

    public List<TenantViewModel> Tenants { get; private set; } = [];

    public async Task<List<TenantViewModel>> GetTenants(
        CancellationToken cancellationToken)
    {
        Tenants.Clear();

        try
        {
            var credential = new InteractiveBrowserCredential();
            var authenticationRecord = await credential.AuthenticateAsync(cancellationToken).ConfigureAwait(false);

            var armClient = new ArmClient(credential);

            var result = new List<TenantViewModel>();
            await foreach (var tenant in armClient.GetTenants().GetAllAsync().ConfigureAwait(false))
            {
                if (tenant.Data.TenantId is null)
                {
                    continue;
                }

                result.Add(new TenantViewModel
                {
                    DisplayName = tenant.Data.DisplayName,
                    TenantId = tenant.Data.TenantId!.Value,
                    IsCurrent = tenant.Data.TenantId!.Value.ToString() == authenticationRecord.TenantId,
                });
            }

            Tenants = result
                .OrderBy(x => x.DisplayName)
                .ToList();
        }
        catch (Exception ex)
        {
            // TODO: XXX
        }

        return Tenants;
    }

    /// <summary>
    /// Attempts to sign the user in.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication result.</returns>
    public async Task<User?> SignIn(
        string tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
            {
                TenantId = tenantId,
            }); // TODO: Verify

            try
            {
                var authenticationRecord = await credential.AuthenticateAsync(new TokenRequestContext(DefaultScopes), cancellationToken).ConfigureAwait(false);

                var accessToken = credential.GetToken(new TokenRequestContext(DefaultScopes), cancellationToken);

                return null;
                //authenticationResult = await AcquireTokenSilent(DefaultScopes, accountToLogin);
                //return User;
            }
            catch (MsalUiRequiredException)
            {
                // Cannot log in silently - most likely Azure AD would show a consent dialog or the user needs to re-enter credentials
                //authenticationResult = await AcquireTokenInteractive(windowHandle, cancellationToken, forceLogin: false, accountToLogin);
                //return User;
                return null;
            }
        }
        catch (MsalUiRequiredException)
        {
            return null;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }
}