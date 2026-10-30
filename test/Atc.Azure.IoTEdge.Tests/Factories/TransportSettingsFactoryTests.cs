namespace Atc.Azure.IoTEdge.Tests.Factories;

public sealed class TransportSettingsFactoryTests
{
    [Fact]
    public void BuildMqttTransportSettings_ConnectsOverTcp()
        => TransportSettingsFactory.BuildMqttTransportSettings().GetTransportType().Should().Be(TransportType.Mqtt_Tcp_Only);

    [Fact]
    public void BuildMqttWebSocketTransportSettings_ConnectsOverWebSockets()
        => TransportSettingsFactory.BuildMqttWebSocketTransportSettings().GetTransportType().Should().Be(TransportType.Mqtt_WebSocket_Only);

    [Fact]
    public void BuildAmqpTransportSettings_ConnectsOverTcp()
        => TransportSettingsFactory.BuildAmqpTransportSettings().GetTransportType().Should().Be(TransportType.Amqp_Tcp_Only);

    [Fact]
    public void BuildAmqpWebSocketTransportSettings_ConnectsOverWebSockets()
        => TransportSettingsFactory.BuildAmqpWebSocketTransportSettings().GetTransportType().Should().Be(TransportType.Amqp_WebSocket_Only);
}