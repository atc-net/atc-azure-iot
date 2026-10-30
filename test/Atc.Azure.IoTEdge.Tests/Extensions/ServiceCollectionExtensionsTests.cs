namespace Atc.Azure.IoTEdge.Tests.Extensions;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddModuleClientWrapper_RegistersTheModuleClientWrapperAsASingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddModuleClientWrapper(TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.Validate));

        // Assert
        services.Should().ContainSingle(x => x.ServiceType.Equals(typeof(IModuleClientWrapper)))
            .Which.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddModuleClientWrapper_SettingsFactory_IsOnlyCalledWhenTheModuleClientIsCreated()
    {
        // Arrange
        var services = new ServiceCollection();
        IServiceProvider? calledWith = null;

        services.AddModuleClientWrapper(serviceProvider =>
        {
            calledWith = serviceProvider;
            return TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.Validate);
        });

        using var serviceProvider = services.BuildServiceProvider();

        // Act
        // Outside IoT Edge the device SDK can't create the module client, but the settings are created first.
        var act = () => serviceProvider.GetRequiredService<IModuleClientWrapper>();

        // Assert
        calledWith.Should().BeNull();
        act.Should().Throw<Exception>();
        calledWith.Should().NotBeNull();
    }
}