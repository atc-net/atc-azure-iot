namespace Atc.Azure.IoT.Wpf.App.UserControls;

/// <summary>
/// Interaction logic for AzureTenantSelectionView.
/// </summary>
public partial class AzureTenantSelectionView
{
    public AzureTenantSelectionView()
    {
        InitializeComponent();
    }

    private void OnTenantSelectedChangedHandler(
        object? sender,
        ValueChangedEventArgs<string?> e)
    {
        var vm = DataContext as AzureTenantSelectionViewModel;
        vm!.OnTenantSelectedChangedHandler(sender, e);
    }
}