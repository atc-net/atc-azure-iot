namespace Atc.Azure.IoT.Tests.Services;

/// <summary>
/// The services check their connection string when they are created (<c>ServiceBase.ValidateAndAssign</c>).
/// </summary>
public sealed class ConnectionStringValidationTests
{
    [Fact]
    public void IoTHubService_ValidConnectionString_IsCreated()
    {
        // Act
        using var actual = new IoTHubService(
            Substitute.For<IIoTHubModuleService>(),
            new IotHubOptions { ConnectionString = TestConnectionStrings.IotHub });

        // Assert
        actual.Should().NotBeNull();
    }

    [Fact]
    public void IoTHubService_NoConnectionString_ThrowsArgumentException()
    {
        // Act
        var act = () => new IoTHubService(
            Substitute.For<IIoTHubModuleService>(),
            new IotHubOptions { ConnectionString = string.Empty });

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IoTHubService_TextIsNotAConnectionString_ThrowsInvalidConfigurationException()
    {
        // Act
        var act = () => new IoTHubService(
            Substitute.For<IIoTHubModuleService>(),
            new IotHubOptions { ConnectionString = "not a connection string" });

        // Assert
        act.Should().Throw<InvalidConfigurationException>()
            .WithMessage("Invalid service configuration for ConnectionString*");
    }

    [Fact]
    public void IoTHubService_KeyIsNotBase64_ThrowsInvalidConfigurationExceptionWithoutTheKeyInTheMessage()
    {
        // Arrange
        const string invalidKey = "this-key-is-not-base64";

        // Act
        var act = () => new IoTHubService(
            Substitute.For<IIoTHubModuleService>(),
            new IotHubOptions { ConnectionString = $"HostName=test-hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey={invalidKey}" });

        // Assert
        act.Should().Throw<InvalidConfigurationException>()
            .Which.Message.Should().NotContain(invalidKey);
    }

    [Fact]
    public void IoTHubModuleService_TextIsNotAConnectionString_ThrowsInvalidConfigurationException()
    {
        // Act
        var act = () => new IoTHubModuleService(new IotHubOptions { ConnectionString = "not a connection string" });

        // Assert
        act.Should().Throw<InvalidConfigurationException>();
    }

    [Fact]
    public void DeviceProvisioningService_ValidConnectionString_IsCreated()
    {
        // Act
        using var actual = new DeviceProvisioningService(
            new DeviceProvisioningServiceOptions { ConnectionString = TestConnectionStrings.DeviceProvisioningService });

        // Assert
        actual.Should().NotBeNull();
    }

    [Fact]
    public void DeviceProvisioningService_TextIsNotAConnectionString_ThrowsInvalidConfigurationException()
    {
        // Act
        var act = () => new DeviceProvisioningService(
            new DeviceProvisioningServiceOptions { ConnectionString = "not a connection string" });

        // Assert
        act.Should().Throw<InvalidConfigurationException>();
    }
}