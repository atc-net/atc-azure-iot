namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ConfigureIotHubServices_RegistersTheIoTHubServicesTheTwinModuleExtractorAndTheManifestProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureIotHubServices(new IotHubOptions { ConnectionString = TestConnectionStrings.IotHub });

        // Assert
        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<IIoTHubService>().Should().BeOfType<IoTHubService>();
        serviceProvider.GetRequiredService<IIoTHubModuleService>().Should().BeOfType<IoTHubModuleService>();
        serviceProvider.GetRequiredService<IDeviceTwinModuleExtractor>().Should().BeOfType<DeviceTwinModuleExtractor>();
        serviceProvider.GetRequiredService<IConfigurationContentProvider>().Should().BeOfType<ConfigurationContentProvider>();
    }

    [Fact]
    public void ConfigureIotHubServices_NoConnectionString_ThrowsInvalidOperationException()
    {
        // Act
        var act = () => new ServiceCollection().ConfigureIotHubServices(new IotHubOptions());

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("IotHubOptions.ConnectionString is missing.");
    }

    [Fact]
    public void ConfigureIotHubServices_LoggerFactoryResolverGiven_UsesItsLoggerFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        var resolverCalls = 0;

        services.ConfigureIotHubServices(
            new IotHubOptions { ConnectionString = TestConnectionStrings.IotHub },
            _ =>
            {
                resolverCalls++;
                return null;
            });

        using var serviceProvider = services.BuildServiceProvider();

        // Act
        serviceProvider.GetRequiredService<IIoTHubService>();

        // Assert
        // Once for IoTHubService and once for the IoTHubModuleService it depends on.
        resolverCalls.Should().Be(2);
    }

    [Fact]
    public void ConfigureIotHubServicesAndConfigureDeviceProvisioningServices_RegisterTheManifestProviderOnce()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services
            .ConfigureIotHubServices(new IotHubOptions { ConnectionString = TestConnectionStrings.IotHub })
            .ConfigureDeviceProvisioningServices(new DeviceProvisioningServiceOptions { ConnectionString = TestConnectionStrings.DeviceProvisioningService });

        // Assert
        services.Count(x => x.ServiceType == typeof(IConfigurationContentProvider)).Should().Be(1);
    }

    [Fact]
    public void ConfigureDeviceProvisioningServices_RegistersTheDpsServiceAndTheManifestProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureDeviceProvisioningServices(new DeviceProvisioningServiceOptions { ConnectionString = TestConnectionStrings.DeviceProvisioningService });

        // Assert
        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<IDeviceProvisioningService>().Should().BeOfType<DeviceProvisioningService>();
        serviceProvider.GetRequiredService<IConfigurationContentProvider>().Should().BeOfType<ConfigurationContentProvider>();
    }

    [Fact]
    public void ConfigureDeviceProvisioningServices_NoConnectionString_ThrowsInvalidOperationException()
    {
        // Act
        var act = () => new ServiceCollection().ConfigureDeviceProvisioningServices(new DeviceProvisioningServiceOptions());

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("DeviceProvisioningServiceOptions.ConnectionString is missing.");
    }
}