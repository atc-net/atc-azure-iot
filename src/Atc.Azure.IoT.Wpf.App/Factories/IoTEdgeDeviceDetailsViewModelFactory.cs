// ReSharper disable InvertIf
namespace Atc.Azure.IoT.Wpf.App.Factories;

public static class IoTEdgeDeviceDetailsViewModelFactory
{
    public static IoTEdgeDeviceDetailsViewModel Create(
        EdgeAgentReportedProperties edgeAgentReportedProperties)
    {
        var result = new IoTEdgeDeviceDetailsViewModel
        {
            OperatingSystem = edgeAgentReportedProperties.Runtime?.Platform.OperatingSystem ?? "N/A",
            OperatingSystemArchitecture = edgeAgentReportedProperties.Runtime?.Platform.OperatingSystemArchitecture ?? "N/A",
            RuntimeStatusCode = edgeAgentReportedProperties.LastDesiredStatus?.RuntimeStatusCode ?? -1,
            RuntimeStatusDescription = edgeAgentReportedProperties.LastDesiredStatus?.RuntimeStatusDescription ?? "N/A",
        };

        if (edgeAgentReportedProperties.SystemModules is not null &&
            edgeAgentReportedProperties.SystemModules.Count > 0)
        {
            foreach (var module in edgeAgentReportedProperties.SystemModules)
            {
                result.SystemModules.Add(BuildIotEdgeModule(module));
            }
        }

        if (edgeAgentReportedProperties.Modules is not null &&
            edgeAgentReportedProperties.Modules.Count > 0)
        {
            foreach (var module in edgeAgentReportedProperties.Modules)
            {
                result.CustomModules.Add(BuildIotEdgeModule(module));
            }
        }

        return result;
    }

    private static IoTEdgeModuleViewModel BuildIotEdgeModule(
        IoT.Models.Module module)
    {
        var result = new IoTEdgeModuleViewModel
        {
            ModuleId = module.Name,
            UpstreamProtocol = module.Environment?.UpstreamProtocol?.Value ?? "N/A",
            StartupOrder = module.StartupOrder,
            ContainerImage = module.Settings.Image,
            State = IotEdgeModuleState.Unknown,
            LastExitTimeUtc = module.LastExitTimeUtc,
            LastStartTimeUtc = module.LastStartTimeUtc,
            LastRestartTimeUtc = module.LastRestartTimeUtc,
            RestartCount = module.RestartCount,
            ExitCode = module.ExitCode,
            StatusDescription = module.StatusDescription,
        };

        if (Enum<IotEdgeModuleState>.TryParse(module.RuntimeStatus, out var edgeModuleState))
        {
            result.State = edgeModuleState;
        }

        return result;
    }
}