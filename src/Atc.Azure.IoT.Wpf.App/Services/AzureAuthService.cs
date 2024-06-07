namespace Atc.Azure.IoT.Wpf.App.Services;

public sealed class AzureAuthService // TODO: interface
{
    public TokenCredential? Credential { get; private set; }

    public AuthenticationRecord? AuthenticationRecord { get; private set; }

    public async Task<(bool Succeeded, string? ErrorMessage)> SignIn(
        CancellationToken cancellationToken)
    {
        var credential = new InteractiveBrowserCredential();

        try
        {
            AuthenticationRecord = await credential
                .AuthenticateAsync(cancellationToken)
                .ConfigureAwait(false);

            Credential = credential;

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> SignInToTenant(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
        {
            TenantId = tenantId.ToString(),
            ////BrowserCustomization = new BrowserCustomizationOptions
            ////{
            ////    UseEmbeddedWebView = true,
            ////}, // TODO: Test - REF: https://github.com/Azure/azure-sdk-for-net/issues/42235
        });

        try
        {
            AuthenticationRecord = await credential
                .AuthenticateAsync(cancellationToken)
                .ConfigureAwait(false);

            Credential = credential;

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}