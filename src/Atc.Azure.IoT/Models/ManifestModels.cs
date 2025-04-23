namespace Atc.Azure.IoT.Models;

public record Route(
    string Name,
    string Value,
    int? Priority,
    int? TimeToLiveSecs);

public record RegistryCredential(
    string Name,
    string Address,
    string UserName,
    string Password);

public record EnvironmentVariable(
    string Name,
    string Value);

public record ModuleSpecificationDesiredProperties(
    string Name,
    object DesiredProperties);

public record EdgeModuleSpecification(
    string Name,
    string Image,
    int? StartupOrder,
    List<EnvironmentVariable> EnvironmentVariables,
    string Version = "1.0",
    RestartPolicy RestartPolicy = RestartPolicy.Always,
    string CreateOptions = "",
    ModuleStatus Status = ModuleStatus.Running);

public record EdgeHubDesiredProperties(
    List<Route>? Routes = null,
    string SchemaVersion = "1.1",
    int StoreAndForwardTimeToLiveSecs = 7200);

public record EdgeAgentDesiredProperties(
    List<RegistryCredential> RegistryCredentials,
    List<EdgeModuleSpecification> EdgeSystemModuleSpecifications,
    List<EdgeModuleSpecification> EdgeModuleSpecifications,
    string SchemaVersion = "1.1",
    string SystemModuleVersion = "1.5",
    string MinDockerVersion = "v1.25",
    string EdgeAgentCreateOptions = "",
    string EdgeHubCreateOptions = "{\"HostConfig\":{\"PortBindings\":{\"443/tcp\":[{\"HostPort\":\"443\"}],\"5671/tcp\":[{\"HostPort\":\"5671\"}],\"8883/tcp\":[{\"HostPort\":\"8883\"}]}}}");