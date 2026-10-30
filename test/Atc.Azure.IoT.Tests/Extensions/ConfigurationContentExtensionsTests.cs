// The Atc.Azure.IoT.Models namespace also has a Module type, so this namespace is imported here only.
using Microsoft.Azure.Devices;

namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class ConfigurationContentExtensionsTests
{
    [Fact]
    public void SetEdgeAgent_ReturnsTheSameConfigurationContent()
    {
        // Arrange
        var configurationContent = new ConfigurationContent();

        // Act
        var actual = configurationContent.SetEdgeAgent(CreateEdgeAgent());

        // Assert
        actual.Should().BeSameAs(configurationContent);
    }

    [Fact]
    public void SetEdgeAgent_NoSystemModulesGiven_AddsEdgeAgentAndEdgeHubImagesOfTheSystemModuleVersion()
    {
        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(CreateEdgeAgent(systemModuleVersion: "1.6")));

        // Assert
        var systemModules = Section(desired, "systemModules");
        Section(Section(systemModules, "edgeAgent"), "settings")["image"].Should().Be("mcr.microsoft.com/azureiotedge-agent:1.6");
        Section(Section(systemModules, "edgeHub"), "settings")["image"].Should().Be("mcr.microsoft.com/azureiotedge-hub:1.6");
        Section(systemModules, "edgeHub")["status"].Should().Be("running");
        Section(systemModules, "edgeHub")["restartPolicy"].Should().Be("always");
    }

    [Fact]
    public void SetEdgeAgent_SystemModulesGiven_UsesThemInsteadOfTheDefaultImages()
    {
        // Arrange
        var edgeAgent = CreateEdgeAgent(
            systemModules:
            [
                new EdgeModuleSpecification("edgeAgent", "registry.example/agent:1.0", StartupOrder: null, []),
                new EdgeModuleSpecification("edgeHub", "registry.example/hub:1.0", StartupOrder: null, []),
            ]);

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(edgeAgent));

        // Assert
        var systemModules = Section(desired, "systemModules");
        Section(Section(systemModules, "edgeAgent"), "settings")["image"].Should().Be("registry.example/agent:1.0");
        Section(Section(systemModules, "edgeHub"), "settings")["image"].Should().Be("registry.example/hub:1.0");
    }

    [Fact]
    public void SetEdgeAgent_SystemModulesGiven_EdgeAgentHasNoStatusAndEdgeHubHasNoVersion()
    {
        // Arrange
        var edgeAgent = CreateEdgeAgent(
            systemModules:
            [
                new EdgeModuleSpecification("edgeAgent", "agent:1.0", StartupOrder: null, []),
                new EdgeModuleSpecification("edgeHub", "hub:1.0", StartupOrder: null, []),
            ]);

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(edgeAgent));

        // Assert
        var systemModules = Section(desired, "systemModules");
        Section(systemModules, "edgeAgent").Keys.Should().BeEquivalentTo("type", "settings");
        Section(systemModules, "edgeHub").Keys.Should().BeEquivalentTo("type", "status", "restartPolicy", "settings");
    }

    [Fact]
    public void SetEdgeAgent_RegistryCredentialsGiven_AddsThemToTheRuntimeSettings()
    {
        // Arrange
        var edgeAgent = CreateEdgeAgent(registryCredentials: [new RegistryCredential("myregistry", "myregistry.azurecr.io", "user", "secret")]);

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(edgeAgent));

        // Assert
        var credentials = (Dictionary<string, Dictionary<string, string>>)Section(Section(desired, "runtime"), "settings")["registryCredentials"];
        credentials["myregistry"].Should().BeEquivalentTo(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["address"] = "myregistry.azurecr.io",
            ["username"] = "user",
            ["password"] = "secret",
        });
    }

    [Fact]
    public void SetEdgeAgent_NoModulesGiven_LeavesOutTheModulesSection()
    {
        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(CreateEdgeAgent()));

        // Assert
        desired.Should().NotContainKey("modules");
    }

    [Fact]
    public void SetEdgeAgent_ModuleGiven_AddsItWithVersionStatusRestartPolicyStartupOrderAndEnvironment()
    {
        // Arrange
        var module = new EdgeModuleSpecification(
            "simulation",
            "simulation:2.0",
            StartupOrder: 2,
            [new EnvironmentVariable("Interval", "5")],
            Version: "2.0",
            RestartPolicy: RestartPolicy.Never,
            Status: ModuleStatus.Stopped);

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(CreateEdgeAgent(modules: [module])));

        // Assert
        var simulation = Section(Section(desired, "modules"), "simulation");
        simulation["type"].Should().Be("docker");
        simulation["version"].Should().Be("2.0");
        simulation["status"].Should().Be("stopped");
        simulation["restartPolicy"].Should().Be("never");
        simulation["startupOrder"].Should().Be(2);
        Section(simulation, "settings")["image"].Should().Be("simulation:2.0");
        Newtonsoft.Json.JsonConvert.SerializeObject(Section(simulation, "env")).Should().Be("""{"Interval":{"value":"5"}}""");
    }

    [Fact]
    public void SetEdgeAgent_ModuleCreateOptionsAreJson_KeepsThemAsJson()
    {
        // Arrange
        var module = new EdgeModuleSpecification("simulation", "simulation:1.0", StartupOrder: null, [], CreateOptions: """{"HostConfig":{"Privileged":true}}""");

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(CreateEdgeAgent(modules: [module])));

        // Assert
        var createOptions = Section(Section(Section(desired, "modules"), "simulation"), "settings")["createOptions"];
        createOptions.Should().BeAssignableTo<Newtonsoft.Json.Linq.JToken>()
            .Which.ToString(Newtonsoft.Json.Formatting.None).Should().Be("""{"HostConfig":{"Privileged":true}}""");
    }

    [Fact]
    public void SetEdgeAgent_ModuleCreateOptionsAreNotJson_KeepsThemAsText()
    {
        // Arrange
        var module = new EdgeModuleSpecification("simulation", "simulation:1.0", StartupOrder: null, [], CreateOptions: "not json");

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(CreateEdgeAgent(modules: [module])));

        // Assert
        Section(Section(Section(desired, "modules"), "simulation"), "settings")["createOptions"].Should().Be("not json");
    }

    [Fact]
    public void SetEdgeAgent_ModuleWithoutCreateOptionsOrStartupOrderOrEnvironment_LeavesThemOut()
    {
        // Arrange
        var module = new EdgeModuleSpecification("simulation", "simulation:1.0", StartupOrder: null, []);

        // Act
        var desired = EdgeAgentDesired(new ConfigurationContent().SetEdgeAgent(CreateEdgeAgent(modules: [module])));

        // Assert
        var simulation = Section(Section(desired, "modules"), "simulation");
        simulation.Should().NotContainKeys("startupOrder", "env");
        Section(simulation, "settings").Should().NotContainKey("createOptions");
    }

    [Fact]
    public void SetEdgeHub_RouteWithoutPriorityOrTimeToLive_UsesTheShortRouteFormat()
    {
        // Arrange
        var edgeHub = new EdgeHubDesiredProperties([new Route("upstream", "FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null)]);

        // Act
        var desired = EdgeHubDesired(new ConfigurationContent().SetEdgeHub(edgeHub));

        // Assert
        Section(desired, "routes")["upstream"].Should().Be("FROM /messages/* INTO $upstream");
    }

    [Fact]
    public void SetEdgeHub_RouteWithPriorityAndTimeToLive_UsesTheRouteObjectFormat()
    {
        // Arrange
        var edgeHub = new EdgeHubDesiredProperties([new Route("alarms", "FROM /messages/* INTO $upstream", Priority: 0, TimeToLiveSecs: 86400)]);

        // Act
        var desired = EdgeHubDesired(new ConfigurationContent().SetEdgeHub(edgeHub));

        // Assert
        Section(Section(desired, "routes"), "alarms").Should().BeEquivalentTo(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["route"] = "FROM /messages/* INTO $upstream",
            ["priority"] = 0,
            ["timeToLiveSecs"] = 86400,
        });
    }

    [Fact]
    public void SetEdgeHub_SetsTheSchemaVersionAndTheStoreAndForwardTimeToLive()
    {
        // Act
        var desired = EdgeHubDesired(new ConfigurationContent().SetEdgeHub(new EdgeHubDesiredProperties(SchemaVersion: "1.2", StoreAndForwardTimeToLiveSecs: 600)));

        // Assert
        desired["schemaVersion"].Should().Be("1.2");
        Section(desired, "storeAndForwardConfiguration")["timeToLiveSecs"].Should().Be(600);
        Section(desired, "routes").Should().BeEmpty();
    }

    [Fact]
    public void SetModuleDesiredProperty_AddsTheDesiredPropertiesUnderTheModuleName()
    {
        // Arrange
        var desiredProperties = new { interval = 5 };

        // Act
        var actual = new ConfigurationContent().SetModuleDesiredProperty(new ModuleSpecificationDesiredProperties("simulation", desiredProperties));

        // Assert
        actual.ModulesContent["simulation"]["properties.desired"].Should().BeSameAs(desiredProperties);
    }

    [Fact]
    public void SetEdgeAgentAndSetEdgeHub_ResultCanBeReadBackAsADeploymentManifest()
    {
        // Arrange
        var configurationContent = new ConfigurationContent()
            .SetEdgeAgent(CreateEdgeAgent(modules: [new EdgeModuleSpecification("simulation", "simulation:1.0", StartupOrder: 1, [])]))
            .SetEdgeHub(new EdgeHubDesiredProperties([new Route("upstream", "FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null)]));

        // Act
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(configurationContent);
        var actual = JsonSerializer.Deserialize<DeploymentManifest>(json);

        // Assert
        actual.Should().NotBeNull();
        actual!.ModulesContent.EdgeAgent.PropertiesDesired.Modules.Should().ContainKey("simulation");
        actual.ModulesContent.EdgeHub.PropertiesDesired.Routes.Should().ContainKey("upstream");
    }

    private static EdgeAgentDesiredProperties CreateEdgeAgent(
        List<RegistryCredential>? registryCredentials = null,
        List<EdgeModuleSpecification>? systemModules = null,
        List<EdgeModuleSpecification>? modules = null,
        string systemModuleVersion = "1.5")
        => new(
            registryCredentials ?? [],
            systemModules ?? [],
            modules ?? [],
            SystemModuleVersion: systemModuleVersion);

    private static Dictionary<string, object> EdgeAgentDesired(ConfigurationContent configurationContent)
        => (Dictionary<string, object>)configurationContent.ModulesContent["$edgeAgent"]["properties.desired"];

    private static Dictionary<string, object> EdgeHubDesired(ConfigurationContent configurationContent)
        => (Dictionary<string, object>)configurationContent.ModulesContent["$edgeHub"]["properties.desired"];

    private static Dictionary<string, object> Section(
        IDictionary<string, object> parent,
        string name)
        => (Dictionary<string, object>)parent[name];
}