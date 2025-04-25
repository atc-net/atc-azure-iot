namespace Atc.Azure.IoT.Models;

public record DeploymentManifest(
    [property: JsonPropertyName(PropertyNames.Manifest.ModulesContent)] ModulesContentDefinition ModulesContent);

public record ModulesContentDefinition(
    [property: JsonPropertyName(EdgeAgentConstants.ModuleId)] EdgeAgent EdgeAgent,
    [property: JsonPropertyName(EdgeHubConstants.ModuleId)] EdgeHub EdgeHub);

public record EdgeAgent(
    [property: JsonPropertyName(PropertyNames.Manifest.PropertiesDesired)] PropertiesDesiredEdgeAgent PropertiesDesired);

public record PropertiesDesiredEdgeAgent(
    [property: JsonPropertyName(PropertyNames.Manifest.SchemaVersion)] string SchemaVersion,
    [property: JsonPropertyName(PropertyNames.Runtime)] EdgeAgentRuntime Runtime,
    [property: JsonPropertyName(PropertyNames.SystemModules)] Dictionary<string, SystemModuleSpecification> SystemModules,
    [property: JsonPropertyName(PropertyNames.Modules)] Dictionary<string, SystemModuleSpecification> Modules);

public record EdgeAgentRuntime(
    [property: JsonPropertyName(PropertyNames.Manifest.Type)] string Type,
    [property: JsonPropertyName(PropertyNames.ModuleSettings)] Settings Settings);

public record Settings(
    [property: JsonPropertyName(PropertyNames.Manifest.MinDockerVersion)] string MinDockerVersion,
    [property: JsonPropertyName(PropertyNames.Manifest.LoggingOptions)] string LoggingOptions,
    [property: JsonPropertyName(PropertyNames.Manifest.RegistryCredentials)] Dictionary<string, Store> RegistryCredentials);

public record Store(
    [property: JsonPropertyName(PropertyNames.Manifest.Address)] string Address,
    [property: JsonPropertyName(PropertyNames.Manifest.Password)] string Password,
    [property: JsonPropertyName(PropertyNames.Manifest.Username)] string Username);

public record SystemModuleSpecification(
    [property: JsonPropertyName(PropertyNames.Manifest.Type)] string Type,
    [property: JsonPropertyName(PropertyNames.Manifest.Status)] string Status,
    [property: JsonPropertyName(PropertyNames.Manifest.RestartPolicy)] string RestartPolicy,
    [property: JsonPropertyName(PropertyNames.ModuleSettings)] SettingsSpecification Settings,
    [property: JsonPropertyName(PropertyNames.ModuleEnvironment)] Dictionary<string, EnvSpecification> Env,
    [property: JsonPropertyName(PropertyNames.ModuleStartupOrder)] int? StartupOrder);

public record SettingsSpecification(
    [property: JsonPropertyName(PropertyNames.Manifest.Image)] string Image,
    [property: JsonPropertyName(PropertyNames.Manifest.CreateOptions)] string CreateOptions);

public record EnvSpecification(
    [property: JsonPropertyName(PropertyNames.Manifest.Value)] string Value);

public record EdgeHub(
    [property: JsonPropertyName(PropertyNames.Manifest.PropertiesDesired)] PropertiesDesiredEdgeHub PropertiesDesired);

public record PropertiesDesiredEdgeHub(
    [property: JsonPropertyName(PropertyNames.Manifest.SchemaVersion)] string SchemaVersion,
    [property: JsonPropertyName(PropertyNames.Manifest.Routes)] Dictionary<string, EdgeHubRoute>? Routes,
    [property: JsonPropertyName(PropertyNames.Manifest.StoreAndForwardConfiguration)] StoreAndForwardConfiguration StoreAndForwardConfiguration);

[Newtonsoft.Json.JsonConverter(typeof(EdgeHubRouteConverter))]
public record EdgeHubRoute(
    [property: JsonPropertyName(PropertyNames.Manifest.Route)] string Route,
    [property: JsonPropertyName(PropertyNames.Manifest.Priority)] int? Priority,
    [property: JsonPropertyName(PropertyNames.Manifest.TimeToLiveSecs)] int? TimeToLiveSecs);

public record StoreAndForwardConfiguration(
    [property: JsonPropertyName(PropertyNames.Manifest.TimeToLiveSecs)] int TimeToLiveSecs);