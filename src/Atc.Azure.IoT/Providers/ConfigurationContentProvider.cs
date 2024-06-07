namespace Atc.Azure.IoT.Providers;

public sealed class ConfigurationContentProvider : IConfigurationContentProvider
{
    public async Task<(ConfigurationContent? ConfigurationContent, string? ErrorMessage)> GetConfigurationContent(
        FileInfo deploymentManifestFileInfo,
        CancellationToken cancellationToken)
    {
        if (!deploymentManifestFileInfo.Exists)
        {
            return (null, "Deployment Manifest file does not exist.");
        }

        try
        {
            await using var fileStream = deploymentManifestFileInfo.OpenRead();
            return await GetConfigurationContent(fileStream, cancellationToken);
        }
        catch (Exception ex)
        {
            return (null, $"Error reading Deployment Manifest file: {ex.Message}");
        }
    }

    public async Task<(ConfigurationContent? ConfigurationContent, string? ErrorMessage)> GetConfigurationContent(
        Stream deploymentManifestFileStream,
        CancellationToken cancellationToken)
    {
        var templateContent = await GetTemplateContent(deploymentManifestFileStream, cancellationToken);
        if (templateContent is null)
        {
            return (null, "Deployment Manifest was not in proper format.");
        }

        var configurationContent = GetConfigurationContentFromManifest(templateContent!);
        if (configurationContent is null)
        {
            return (null, "Could not get ConfigurationContent from Deployment Manifest.");
        }

        return (configurationContent, null);
    }

    private static async Task<string?> GetTemplateContent(
        Stream deploymentManifestFileStream,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(deploymentManifestFileStream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        return content.IsFormatJson()
            ? content
            : null;
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
    private static ConfigurationContent? GetConfigurationContentFromManifest(
        string deploymentManifest)
        => Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationContent>(
            deploymentManifest);
}