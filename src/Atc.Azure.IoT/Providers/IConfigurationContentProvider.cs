namespace Atc.Azure.IoT.Providers;

public interface IConfigurationContentProvider
{
    Task<(ConfigurationContent? ConfigurationContent, string? ErrorMessage)> GetConfigurationContent(
        FileInfo deploymentManifestFileInfo,
        CancellationToken cancellationToken);

    Task<(ConfigurationContent? ConfigurationContent, string? ErrorMessage)> GetConfigurationContent(
        Stream deploymentManifestFileStream,
        CancellationToken cancellationToken);
}