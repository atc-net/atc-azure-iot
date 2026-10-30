namespace Atc.Azure.IoT.Providers;

public sealed class ConfigurationContentProvider : IConfigurationContentProvider
{
    /// <inheritdoc />
    public async Task<ConfigurationContent> GetConfigurationContent(
        FileInfo deploymentManifestFileInfo,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(deploymentManifestFileInfo);

        await using var fileStream = deploymentManifestFileInfo.OpenRead();
        return await GetConfigurationContent(fileStream, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ConfigurationContent> GetConfigurationContent(
        Stream deploymentManifestFileStream,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(deploymentManifestFileStream);

        using var reader = new StreamReader(deploymentManifestFileStream, leaveOpen: true);
        var deploymentManifest = await reader.ReadToEndAsync(cancellationToken);

        return GetConfigurationContentFromManifest(deploymentManifest);
    }

    /// <summary>
    /// Gets the ConfigurationContent from the deployment manifest
    /// </summary>
    /// <param name="deploymentManifest">The deployment manifest</param>
    /// <returns>The deployment manifest as a <see cref="ConfigurationContent"/></returns>
    /// <remarks>
    /// We utilize Newtonsoft.Json here, because System.Text.Json does not work!
    /// When de-serializing the edgeAgent - properties.desired, instead of "object-array", the serializer returns:
    /// ValueKind = Object : " instead of {
    /// and " instead of } (for the end).
    /// </remarks>
    private static ConfigurationContent GetConfigurationContentFromManifest(
        string deploymentManifest)
    {
        ConfigurationContent? configurationContent;

        try
        {
            configurationContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationContent>(deploymentManifest);
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException($"The deployment manifest is not a valid JSON object: {ex.Message}", ex);
        }

        return configurationContent
               ?? throw new InvalidDataException("The deployment manifest is empty.");
    }
}