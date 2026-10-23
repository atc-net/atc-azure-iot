namespace Atc.Aspire.Hosting.Azure.IoTEdge;

/// <summary>
/// Represents an emulated IoT Edge device: an edge-enabled device identity in a real Azure IoT Hub,
/// with a module identity for every Aspire resource attached through <c>AsIoTEdgeModule</c>.
/// </summary>
public sealed class IoTEdgeDeviceResource : Resource, IResourceWithWaitSupport
{
    private readonly TaskCompletionSource<IoTEdgeDeviceIdentity> provisioning = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly List<IoTEdgeModuleAnnotation> modules = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="IoTEdgeDeviceResource"/> class.
    /// </summary>
    /// <param name="name">The resource name.</param>
    /// <param name="iotHubConnectionString">The parameter holding the IoT Hub service connection string.</param>
    /// <param name="deviceId">The id of the edge device in IoT Hub.</param>
    /// <param name="certificateDirectory">The directory holding the edgeHub test certificates.</param>
    public IoTEdgeDeviceResource(
        string name,
        ParameterResource iotHubConnectionString,
        string deviceId,
        string certificateDirectory)
        : base(name)
    {
        ArgumentNullException.ThrowIfNull(iotHubConnectionString);
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(certificateDirectory);

        IotHubConnectionString = iotHubConnectionString;
        DeviceId = deviceId;
        CertificateDirectory = certificateDirectory;
    }

    /// <summary>
    /// Gets the parameter holding the IoT Hub service connection string used to provision the device.
    /// The policy needs registry read/write, and service connect for the dashboard commands.
    /// </summary>
    public ParameterResource IotHubConnectionString { get; }

    /// <summary>
    /// Gets the id of the edge device in IoT Hub.
    /// </summary>
    public string DeviceId { get; internal set; }

    /// <summary>
    /// Gets the directory holding the edgeHub test certificates.
    /// </summary>
    public string CertificateDirectory { get; }

    /// <summary>
    /// Gets the path of the deployment manifest whose routes and module twin desired properties are applied, if any.
    /// </summary>
    public string? DeploymentManifestPath { get; internal set; }

    /// <summary>
    /// Gets the local edgeHub container, or <see langword="null"/> when modules connect directly to IoT Hub.
    /// </summary>
    public IoTEdgeHubResource? EdgeHub { get; internal set; }

    /// <summary>
    /// Gets the modules attached to the device.
    /// </summary>
    public IReadOnlyList<IoTEdgeModuleAnnotation> Modules => modules;

    internal IIoTHubService? IoTHubService { get; set; }

    internal IIoTHubModuleService? IoTHubModuleService { get; set; }

    internal DeploymentManifest? DeploymentManifest { get; set; }

    /// <summary>
    /// Waits until the device and its module identities are provisioned in IoT Hub.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The provisioned identities.</returns>
    public Task<IoTEdgeDeviceIdentity> WaitForProvisioningAsync(CancellationToken cancellationToken = default)
        => provisioning.Task.WaitAsync(cancellationToken);

    internal void AddModule(IoTEdgeModuleAnnotation module)
    {
        if (modules.Exists(x => string.Equals(x.ModuleId, module.ModuleId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"Module '{module.ModuleId}' is already attached to IoT Edge device '{Name}'.");
        }

        modules.Add(module);
    }

    internal void SetProvisioned(IoTEdgeDeviceIdentity identity)
        => provisioning.TrySetResult(identity);

    internal void SetProvisioningFailed(Exception exception)
        => provisioning.TrySetException(exception);
}