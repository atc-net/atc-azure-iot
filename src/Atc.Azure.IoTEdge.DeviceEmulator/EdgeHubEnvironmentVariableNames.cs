namespace Atc.Azure.IoTEdge.DeviceEmulator;

/// <summary>
/// Environment variable names understood by the edgeHub container and by the IoT device SDK
/// when a module runs outside the IoT Edge runtime.
/// </summary>
public static class EdgeHubEnvironmentVariableNames
{
    /// <summary>
    /// The module connection string read by <c>ModuleClient.CreateFromEnvironmentAsync</c>.
    /// When set, the SDK bypasses the IoT Edge workload API (IOTEDGE_* variables).
    /// </summary>
    public const string EdgeHubConnectionString = "EdgeHubConnectionString";

    /// <summary>
    /// The CA certificate (PEM) that modules use to trust the edgeHub server certificate.
    /// </summary>
    public const string EdgeModuleCaCertificateFile = "EdgeModuleCACertificateFile";

    /// <summary>
    /// The $edgeHub module connection string used by the edgeHub container to connect upstream.
    /// </summary>
    public const string IotHubConnectionString = "IotHubConnectionString";

    /// <summary>
    /// The edgeHub server certificate (PFX without password).
    /// </summary>
    public const string EdgeModuleHubServerCertificateFile = "EdgeModuleHubServerCertificateFile";

    /// <summary>
    /// The edgeHub server CA chain (PEM).
    /// </summary>
    public const string EdgeModuleHubServerCaChainCertificateFile = "EdgeModuleHubServerCAChainCertificateFile";

    /// <summary>
    /// Selects where edgeHub reads its configuration from. Anything other than <c>twin</c> means local configuration.
    /// </summary>
    public const string ConfigSource = "configSource";

    /// <summary>
    /// The value for <see cref="ConfigSource"/> that makes edgeHub read routes from environment variables.
    /// </summary>
    public const string ConfigSourceLocal = "local";

    /// <summary>
    /// The prefix for route environment variables, e.g. <c>routes__upstream=FROM /messages/* INTO $upstream</c>.
    /// </summary>
    public const string RoutePrefix = "routes__";

    /// <summary>
    /// The name of the default route in the edgeHub image (<c>FROM /* INTO $upstream</c>), which sends every message upstream
    /// on top of the configured routes.
    /// </summary>
    public const string DefaultRouteName = "r1";

    /// <summary>
    /// A route that never matches, used to switch off the default route. edgeHub does not accept an empty route.
    /// </summary>
    public const string DisabledRoute = "FROM /messages/* WHERE $connectionModuleId = '__disabled__' INTO $upstream";

    /// <summary>
    /// The protocol edgeHub uses towards IoT Hub (Amqp, AmqpWs, Mqtt or MqttWs).
    /// </summary>
    public const string UpstreamProtocol = "UpstreamProtocol";
}