namespace Atc.Aspire.Hosting.Azure.IoTEdge;

/// <summary>
/// Represents a local edgeHub container that the modules of an emulated IoT Edge device connect through.
/// </summary>
public sealed class IoTEdgeHubResource : ContainerResource
{
    public const string DefaultRegistry = "mcr.microsoft.com";
    public const string DefaultImage = "azureiotedge-hub";
    public const string DefaultTag = "1.6";

    public const string MqttEndpointName = "mqtt";
    public const string AmqpEndpointName = "amqp";
    public const string HttpsEndpointName = "https";

    public const int MqttPort = 8883;
    public const int AmqpPort = 5671;
    public const int HttpsPort = 443;

    /// <summary>
    /// The directory the certificates are mounted at inside containers.
    /// </summary>
    public const string ContainerCertificateDirectory = "/iotedge/certs";

    public IoTEdgeHubResource(
        string name,
        IoTEdgeDeviceResource device,
        EdgeHubCertificateFiles certificates)
        : base(name)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(certificates);

        Device = device;
        Certificates = certificates;
    }

    /// <summary>
    /// Gets the device this edgeHub belongs to.
    /// </summary>
    public IoTEdgeDeviceResource Device { get; }

    /// <summary>
    /// Gets the edgeHub test certificates on the host.
    /// </summary>
    public EdgeHubCertificateFiles Certificates { get; }

    /// <summary>
    /// Gets the host name containers use to reach edgeHub on the Aspire container network.
    /// </summary>
    public string ContainerHostName => Name;

    /// <summary>
    /// Gets the edgeHub MQTT endpoint.
    /// </summary>
    public EndpointReference MqttEndpoint => new(this, MqttEndpointName);
}