namespace Atc.Azure.IoTEdge.Factories;

/// <summary>
/// Builds the transport settings an IoT Edge module connects to edgeHub with, for
/// <see cref="Extensions.ServiceCollectionExtensions.AddModuleClientWrapper(IServiceCollection, ITransportSettings)"/>.
/// </summary>
/// <remarks>
/// <para>
/// MQTT and AMQP each connect over TCP (MQTT port 8883, AMQP port 5671) or over WebSockets (port 443).
/// </para>
/// <para>
/// Every method takes a <see cref="ServerCertificateValidation"/>, so the choice of how the server certificate is
/// checked is always visible. <see cref="ServerCertificateValidation.Audit"/> also needs a logger for its warnings.
/// </para>
/// </remarks>
public static class TransportSettingsFactory
{
    /// <summary>
    /// Builds AMQP settings that connect over TCP, port 5671.
    /// </summary>
    /// <param name="serverCertificateValidation">How the server certificate is checked.</param>
    /// <param name="logger">The logger for <see cref="ServerCertificateValidation.Audit"/>; not used otherwise.</param>
    /// <returns>The transport settings.</returns>
    /// <exception cref="ArgumentException"><see cref="ServerCertificateValidation.Audit"/> without a logger.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="serverCertificateValidation"/> isn't a defined value.</exception>
    public static AmqpTransportSettings BuildAmqpTransportSettings(
        ServerCertificateValidation serverCertificateValidation,
        ILogger? logger = null)
        => new(TransportType.Amqp_Tcp_Only)
        {
            RemoteCertificateValidationCallback = CreateCertificateValidationCallback(serverCertificateValidation, logger),
        };

    /// <summary>
    /// Builds AMQP settings that connect over WebSockets, port 443, for networks where only that port is open.
    /// </summary>
    /// <inheritdoc cref="BuildAmqpTransportSettings"/>
    public static AmqpTransportSettings BuildAmqpWebSocketTransportSettings(
        ServerCertificateValidation serverCertificateValidation,
        ILogger? logger = null)
        => new(TransportType.Amqp_WebSocket_Only)
        {
            RemoteCertificateValidationCallback = CreateCertificateValidationCallback(serverCertificateValidation, logger),
        };

    /// <summary>
    /// Builds MQTT settings that connect over TCP, port 8883.
    /// </summary>
    /// <inheritdoc cref="BuildAmqpTransportSettings"/>
    public static MqttTransportSettings BuildMqttTransportSettings(
        ServerCertificateValidation serverCertificateValidation,
        ILogger? logger = null)
        => new(TransportType.Mqtt_Tcp_Only)
        {
            RemoteCertificateValidationCallback = CreateCertificateValidationCallback(serverCertificateValidation, logger),
        };

    /// <summary>
    /// Builds MQTT settings that connect over WebSockets, port 443, for networks where only that port is open.
    /// </summary>
    /// <inheritdoc cref="BuildAmqpTransportSettings"/>
    public static MqttTransportSettings BuildMqttWebSocketTransportSettings(
        ServerCertificateValidation serverCertificateValidation,
        ILogger? logger = null)
        => new(TransportType.Mqtt_WebSocket_Only)
        {
            RemoteCertificateValidationCallback = CreateCertificateValidationCallback(serverCertificateValidation, logger),
        };

    /// <summary>
    /// Gets the certificate callback for the transport settings. <see cref="ServerCertificateValidation.Validate"/> has
    /// none: without a callback the device SDK validates the certificate itself, including against the edge CA.
    /// </summary>
    private static RemoteCertificateValidationCallback? CreateCertificateValidationCallback(
        ServerCertificateValidation serverCertificateValidation,
        ILogger? logger)
        => serverCertificateValidation switch
        {
            ServerCertificateValidation.AcceptAny => static (_, _, _, _) => true,
            ServerCertificateValidation.Validate => null,
            ServerCertificateValidation.Audit => CreateAuditCallback(logger),
            _ => throw new ArgumentOutOfRangeException(
                nameof(serverCertificateValidation),
                serverCertificateValidation,
                $"Choose {nameof(ServerCertificateValidation.AcceptAny)}, {nameof(ServerCertificateValidation.Audit)} or {nameof(ServerCertificateValidation.Validate)}."),
        };

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "OK - the edge CA certificate is used by the callback for as long as the module runs.")]
    private static RemoteCertificateValidationCallback CreateAuditCallback(ILogger? logger)
    {
        if (logger is null)
        {
            throw new ArgumentException(
                $"{nameof(ServerCertificateValidation)}.{nameof(ServerCertificateValidation.Audit)} needs a logger for its warnings.",
                nameof(logger));
        }

        var edgeCaCertificate = ServerCertificateAudit.LoadEdgeCaCertificate(
            Environment.GetEnvironmentVariable(ServerCertificateAudit.EdgeCaCertificateFileVariableName),
            logger);

        return ServerCertificateAudit.CreateCallback(logger, edgeCaCertificate);
    }
}