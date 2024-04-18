namespace Atc.Azure.IoTEdge.Factories;

public static class TransportSettingsFactory
{
    public static AmqpTransportSettings BuildEmulatorTransportSettings()
        => new(TransportType.Amqp_Tcp_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };

    public static MqttTransportSettings BuildMqttTransportSettings()
        => new(TransportType.Mqtt_WebSocket_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
}