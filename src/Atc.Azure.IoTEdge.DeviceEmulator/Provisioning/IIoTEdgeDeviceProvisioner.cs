namespace Atc.Azure.IoTEdge.DeviceEmulator.Provisioning;

/// <summary>
/// Provisions the IoT Hub identities an emulated IoT Edge device needs.
/// </summary>
public interface IIoTEdgeDeviceProvisioner
{
    /// <summary>
    /// Ensures an edge-enabled device and the given module identities exist, and applies the module twin
    /// desired properties from the deployment manifest when one is given.
    /// Existing identities are reused, so their keys stay stable between runs.
    /// </summary>
    /// <param name="deviceId">The edge device id.</param>
    /// <param name="moduleIds">The custom module ids to provision.</param>
    /// <param name="deploymentManifest">An optional deployment manifest with module twin desired properties.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The device and module identities.</returns>
    Task<IoTEdgeDeviceIdentity> Provision(
        string deviceId,
        IEnumerable<string> moduleIds,
        DeploymentManifest? deploymentManifest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the device, including its module identities, from IoT Hub.
    /// </summary>
    /// <param name="deviceId">The edge device id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> if the device was deleted; otherwise <see langword="false"/>.</returns>
    Task<bool> Deprovision(
        string deviceId,
        CancellationToken cancellationToken);
}