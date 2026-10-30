namespace Atc.Azure.IoT.Providers;

/// <summary>
/// Reads an IoT Edge deployment manifest into a <see cref="ConfigurationContent"/>, for example to apply it to a device
/// with <see cref="Services.IoTHub.IIoTHubService.ApplyConfigurationContentOnDevice"/>.
/// </summary>
public interface IConfigurationContentProvider
{
    /// <summary>
    /// Reads a deployment manifest file.
    /// </summary>
    /// <param name="deploymentManifestFileInfo">The deployment manifest file.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The deployment manifest's configuration content.</returns>
    /// <exception cref="FileNotFoundException">The file doesn't exist.</exception>
    /// <exception cref="InvalidDataException">The file is empty or isn't a valid JSON object.</exception>
    Task<ConfigurationContent> GetConfigurationContent(
        FileInfo deploymentManifestFileInfo,
        CancellationToken cancellationToken);

    /// <summary>
    /// Reads a deployment manifest from a stream. The stream is read to its end and left open.
    /// </summary>
    /// <param name="deploymentManifestFileStream">The stream holding the deployment manifest.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The deployment manifest's configuration content.</returns>
    /// <exception cref="InvalidDataException">The stream is empty or isn't a valid JSON object.</exception>
    Task<ConfigurationContent> GetConfigurationContent(
        Stream deploymentManifestFileStream,
        CancellationToken cancellationToken);
}