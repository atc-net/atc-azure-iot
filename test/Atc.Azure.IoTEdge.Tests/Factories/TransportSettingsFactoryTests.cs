namespace Atc.Azure.IoTEdge.Tests.Factories;

public sealed class TransportSettingsFactoryTests : IDisposable
{
    private readonly TestCertificates certificates = new();

    public void Dispose()
        => certificates.Dispose();

    [Fact]
    public void BuildMqttTransportSettings_ConnectsOverTcp()
        => TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.Validate)
            .GetTransportType().Should().Be(TransportType.Mqtt_Tcp_Only);

    [Fact]
    public void BuildMqttWebSocketTransportSettings_ConnectsOverWebSockets()
        => TransportSettingsFactory.BuildMqttWebSocketTransportSettings(ServerCertificateValidation.Validate)
            .GetTransportType().Should().Be(TransportType.Mqtt_WebSocket_Only);

    [Fact]
    public void BuildAmqpTransportSettings_ConnectsOverTcp()
        => TransportSettingsFactory.BuildAmqpTransportSettings(ServerCertificateValidation.Validate)
            .GetTransportType().Should().Be(TransportType.Amqp_Tcp_Only);

    [Fact]
    public void BuildAmqpWebSocketTransportSettings_ConnectsOverWebSockets()
        => TransportSettingsFactory.BuildAmqpWebSocketTransportSettings(ServerCertificateValidation.Validate)
            .GetTransportType().Should().Be(TransportType.Amqp_WebSocket_Only);

    [Fact]
    public void BuildMqttTransportSettings_Validate_LeavesTheCheckToTheDeviceSdk()
        => TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.Validate)
            .RemoteCertificateValidationCallback.Should().BeNull();

    [Fact]
    public void BuildAmqpTransportSettings_Validate_LeavesTheCheckToTheDeviceSdk()
        => TransportSettingsFactory.BuildAmqpTransportSettings(ServerCertificateValidation.Validate)
            .RemoteCertificateValidationCallback.Should().BeNull();

    [Fact]
    public void BuildMqttTransportSettings_AcceptAny_AcceptsACertificateThatFailsValidation()
    {
        // Arrange
        var settings = TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.AcceptAny);

        // Act
        var actual = settings.RemoteCertificateValidationCallback!(new object(), certificates.Server, chain: null, SslPolicyErrors.RemoteCertificateNameMismatch);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void BuildAmqpWebSocketTransportSettings_AcceptAny_AcceptsACertificateThatFailsValidation()
    {
        // Arrange
        var settings = TransportSettingsFactory.BuildAmqpWebSocketTransportSettings(ServerCertificateValidation.AcceptAny);

        // Act
        var actual = settings.RemoteCertificateValidationCallback!(new object(), certificates.Server, chain: null, SslPolicyErrors.RemoteCertificateNameMismatch);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void BuildMqttWebSocketTransportSettings_Audit_AcceptsACertificateThatFailsValidationAndLogsAWarning()
    {
        // Arrange
        var logger = new FakeLogger();
        var settings = TransportSettingsFactory.BuildMqttWebSocketTransportSettings(ServerCertificateValidation.Audit, logger);

        // Act
        var actual = settings.RemoteCertificateValidationCallback!(new object(), certificates.Server, chain: null, SslPolicyErrors.RemoteCertificateNameMismatch);

        // Assert
        actual.Should().BeTrue();
        logger.Collector.LatestRecord.Level.Should().Be(LogLevel.Warning);
    }

    [Fact]
    public void BuildMqttTransportSettings_AuditWithoutALogger_ThrowsArgumentException()
    {
        // Act
        var act = () => TransportSettingsFactory.BuildMqttTransportSettings(ServerCertificateValidation.Audit);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("logger");
    }

    [Fact]
    public void BuildMqttTransportSettings_ValidationNeverSet_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => TransportSettingsFactory.BuildMqttTransportSettings(default);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("serverCertificateValidation");
    }
}