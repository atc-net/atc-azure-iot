namespace Atc.Azure.IoT.Extractors;

/// <summary>
/// Provides a mechanism to extract a module from the Edge Agent twin.
/// </summary>
public interface IDeviceTwinModuleExtractor
{
    /// <summary>
    /// Asynchronously extracts a specific module from the Edge Agent twin.
    /// </summary>
    /// <param name="twin">The Edge Agent twin from which to extract the module.</param>
    /// <param name="moduleId">The identifier of the module to extract.</param>
    /// <returns>The module if found; otherwise, null.</returns>
    Task<Models.Module?> GetModuleFromEdgeAgentTwin(
        Twin twin,
        string moduleId);
}