namespace Atc.Azure.IoTEdge.Factories;

/// <summary>
/// Builds the transport settings an IoT Edge module connects to edgeHub with, for
/// <see cref="Extensions.ServiceCollectionExtensions.AddModuleClientWrapper"/>.
/// </summary>
/// <remarks>
/// MQTT and AMQP each connect over TCP (MQTT port 8883, AMQP port 5671) or over WebSockets (port 443).
/// </remarks>
public static class TransportSettingsFactory
{
    /// <summary>
    /// Builds AMQP settings that connect over TCP, port 5671.
    /// </summary>
    /// <returns>The transport settings.</returns>
    public static AmqpTransportSettings BuildAmqpTransportSettings()
        => new(TransportType.Amqp_Tcp_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };

    /// <summary>
    /// Builds AMQP settings that connect over WebSockets, port 443, for networks where only that port is open.
    /// </summary>
    /// <returns>The transport settings.</returns>
    public static AmqpTransportSettings BuildAmqpWebSocketTransportSettings()
        => new(TransportType.Amqp_WebSocket_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };

    /// <summary>
    /// Builds MQTT settings that connect over TCP, port 8883.
    /// </summary>
    /// <returns>The transport settings.</returns>
    public static MqttTransportSettings BuildMqttTransportSettings()
        => new(TransportType.Mqtt_Tcp_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };

    /// <summary>
    /// Builds MQTT settings that connect over WebSockets, port 443, for networks where only that port is open.
    /// </summary>
    /// <returns>The transport settings.</returns>
    public static MqttTransportSettings BuildMqttWebSocketTransportSettings()
        => new(TransportType.Mqtt_WebSocket_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
}