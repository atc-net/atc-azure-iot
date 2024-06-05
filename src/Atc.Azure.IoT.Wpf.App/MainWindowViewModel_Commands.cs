namespace Atc.Azure.IoT.Wpf.App;

public partial class MainWindowViewModel
{
    public IRelayCommand OpenApplicationAboutCommand => new RelayCommand(OpenApplicationAboutCommandHandler);

    public IRelayCommandAsync GetTenantsCommand => new RelayCommandAsync(
        GetTenantsCommandHandler,
        CanGetTenantsCommandHandler);

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