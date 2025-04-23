// ReSharper disable UnusedMember.Global
namespace Atc.Azure.IoT.Models;

public record LastDesiredStatus(
    [property: JsonPropertyName(PropertyNames.DeviceTwin.LastDesiredStatusCode)] int RuntimeStatusCode,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.LastDesiredStatusDescription)] string RuntimeStatusDescription = "");

[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "OK - No Keyword.")]
public record Module(
    [property: JsonIgnore] string Name,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleExitCode)] int ExitCode,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleStatusDescription)] string StatusDescription,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleRuntimeStatus)] string RuntimeStatus,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleLastExitTimeUtc)] DateTimeOffset? LastExitTimeUtc,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleLastStartTimeUtc)] DateTimeOffset? LastStartTimeUtc,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleLastRestartTimeUtc)] DateTimeOffset? LastRestartTimeUtc,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleRestartCount)] int RestartCount,
    [property: JsonPropertyName(PropertyNames.ModuleStartupOrder)] int StartupOrder,
    [property: JsonPropertyName(PropertyNames.ModuleSettings)] ModuleSettings Settings,
    [property: JsonPropertyName(PropertyNames.ModuleEnvironment)] ModuleEnvironment? Environment);

public record ModuleEnvironment(
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleEnvironmentUpstreamProtocol)] UpstreamProtocol? UpstreamProtocol);

public record ModuleSettings(
    [property: JsonPropertyName(PropertyNames.ModuleSettingsImage)] string Image);

public record Platform(
    [property: JsonPropertyName(PropertyNames.DeviceTwin.PlatformOs)] string OperatingSystem,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.PlatformArchitecture)] string OperatingSystemArchitecture);

public record EdgeAgentReportedProperties(
    [property: JsonPropertyName(PropertyNames.Runtime)] Runtime? Runtime,
    [property: JsonPropertyName(PropertyNames.DeviceTwin.LastDesiredStatus)] LastDesiredStatus? LastDesiredStatus,
    [property: JsonPropertyName(PropertyNames.SystemModules), JsonConverter(typeof(JsonModulesConverter))] List<Module>? SystemModules,
    [property: JsonPropertyName(PropertyNames.Modules), JsonConverter(typeof(JsonModulesConverter))] List<Module>? Modules);

[SuppressMessage("Naming", "CA1724:Type Names Should Not Match Namespaces", Justification = "OK - No Keyword.")]
public record Runtime(
    [property: JsonPropertyName(PropertyNames.DeviceTwin.Platform)] Platform Platform);

public record UpstreamProtocol(
    [property: JsonPropertyName(PropertyNames.DeviceTwin.ModuleEnvironmentUpstreamProtocolValue)] string Value);