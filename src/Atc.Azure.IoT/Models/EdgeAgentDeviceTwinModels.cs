// ReSharper disable UnusedMember.Global
namespace Atc.Azure.IoT.Models;

public record EdgeAgentDesiredProperties;

public record LastDesiredStatus(
    [property: JsonPropertyName(DeviceTwinPropertyNames.LastDesiredStatusCode)] int RuntimeStatusCode,
    [property: JsonPropertyName(DeviceTwinPropertyNames.LastDesiredStatusDescription)] string RuntimeStatusDescription = "");

[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "OK - No Keyword.")]
public record Module(
    [property: JsonIgnore] string Name,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleExitCode)] int ExitCode,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleStatusDescription)] string StatusDescription,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleRuntimeStatus)] string RuntimeStatus,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleLastExitTimeUtc)] DateTimeOffset? LastExitTimeUtc,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleLastStartTimeUtc)] DateTimeOffset? LastStartTimeUtc,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleLastRestartTimeUtc)] DateTimeOffset? LastRestartTimeUtc,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleRestartCount)] int RestartCount,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleStartupOrder)] int StartupOrder,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleSettings)] ModuleSettings Settings,
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleEnvironment)] ModuleEnvironment? Environment);

public record ModuleEnvironment(
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleEnvironmentUpstreamProtocol)] UpstreamProtocol? UpstreamProtocol);

public record ModuleSettings(
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleSettingsImage)] string Image);

public record Platform(
    [property: JsonPropertyName(DeviceTwinPropertyNames.PlatformOs)] string OperatingSystem,
    [property: JsonPropertyName(DeviceTwinPropertyNames.PlatformArchitecture)] string OperatingSystemArchitecture);

public record EdgeAgentReportedProperties(
    [property: JsonPropertyName(DeviceTwinPropertyNames.Runtime)] Runtime? Runtime,
    [property: JsonPropertyName(DeviceTwinPropertyNames.LastDesiredStatus)] LastDesiredStatus? LastDesiredStatus,
    [property: JsonPropertyName(DeviceTwinPropertyNames.SystemModules), JsonConverter(typeof(JsonModulesConverter))] List<Module>? SystemModules,
    [property: JsonPropertyName(DeviceTwinPropertyNames.Modules), JsonConverter(typeof(JsonModulesConverter))] List<Module>? Modules);

[SuppressMessage("Naming", "CA1724:Type Names Should Not Match Namespaces", Justification = "OK - No Keyword.")]
public record Runtime(
    [property: JsonPropertyName(DeviceTwinPropertyNames.Platform)] Platform Platform);

public record UpstreamProtocol(
    [property: JsonPropertyName(DeviceTwinPropertyNames.ModuleEnvironmentUpstreamProtocolValue)] string Value);