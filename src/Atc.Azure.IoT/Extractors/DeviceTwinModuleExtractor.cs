namespace Atc.Azure.IoT.Extractors;

public sealed class DeviceTwinModuleExtractor : IDeviceTwinModuleExtractor
{
    public Task<Models.Module?> GetModuleFromEdgeAgentTwin(
        Twin twin,
        string moduleId)
    {
        ArgumentNullException.ThrowIfNull(twin);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        var edgeAgentReportedProperties = twin.GetReportedProperties<EdgeAgentReportedProperties>();

        var module = edgeAgentReportedProperties.SystemModules?.Find(x => x.Name.Equals(moduleId, StringComparison.OrdinalIgnoreCase)) ??
                     edgeAgentReportedProperties.Modules?.Find(x => x.Name.Equals(moduleId, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(module);
    }
}