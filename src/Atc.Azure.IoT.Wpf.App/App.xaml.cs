namespace Atc.Azure.IoT.Wpf.App;

/// <summary>
/// Interaction logic for App.
/// </summary>
public partial class App
{
    private readonly IHost host;
    private IConfiguration? configuration;

    public static readonly BitmapImage DefaultIcon = new(new Uri("pack://application:,,,/Resources/AppIcon.ico", UriKind.Absolute));

    public App()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(
                configurationBuilder =>
                {
                    configuration = configurationBuilder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<AzureResourceStateService>();
                services.AddSingleton<AzureAuthService>();
                services.AddSingleton<AzureResourceManagerService>();
                services.AddSingleton<ToastNotificationManager>();

                services.AddSingleton<StatusBarViewModel>();
                services.AddSingleton<AzureTenantSelectionViewModel>();

                services.AddSingleton<AzureDeviceProvisioningServiceViewModel>();

                services.AddSingleton<AzureIoTHubServiceViewModel>();
                services.AddSingleton<AzureIoTHubSelectorViewModel>();
                services.AddSingleton<AzureIoTHubDeviceSelectorViewModel>();
                services.AddSingleton<AzureIoTHubDeviceViewModel>();

                services.AddSingleton<IMainWindowViewModelBase, MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    private async void ApplicationStartup(
        object sender,
        StartupEventArgs e)
    {
        await host
            .StartAsync()
            .ConfigureAwait(false);

        ThemeManager.Current.ChangeTheme(Current, "Dark.Taupe");

        var mainWindow = host
            .Services
            .GetService<MainWindow>()!;

        mainWindow.Show();
    }

    private async void ApplicationExit(
        object sender,
        ExitEventArgs e)
    {
        await host
            .StopAsync()
            .ConfigureAwait(false);

        host.Dispose();
    }
}