namespace Atc.Azure.IoT.Tests.Extractors;

public sealed class DeviceTwinModuleExtractorTests
{
    private const string ModuleJson =
        """
        {
          "exitCode": 0,
          "statusDescription": "running",
          "runtimeStatus": "running",
          "restartCount": 0,
          "settings": { "image": "image:1.0" }
        }
        """;

    private static readonly Twin EdgeAgentTwin = new()
    {
        Properties = new TwinProperties
        {
            Reported = new TwinCollection(
                $$"""
                {
                  "systemModules": { "edgeAgent": {{ModuleJson}}, "edgeHub": {{ModuleJson}} },
                  "modules": { "simulation": {{ModuleJson}} }
                }
                """),
        },
    };

    private readonly DeviceTwinModuleExtractor sut = new();

    [Fact]
    public async Task GetModuleFromEdgeAgentTwin_SystemModule_ReturnsIt()
    {
        // Act
        var actual = await sut.GetModuleFromEdgeAgentTwin(EdgeAgentTwin, "edgeHub");

        // Assert
        actual.Should().NotBeNull();
        actual!.Name.Should().Be("edgeHub");
    }

    [Fact]
    public async Task GetModuleFromEdgeAgentTwin_CustomModule_ReturnsIt()
    {
        // Act
        var actual = await sut.GetModuleFromEdgeAgentTwin(EdgeAgentTwin, "simulation");

        // Assert
        actual.Should().NotBeNull();
        actual!.Name.Should().Be("simulation");
    }

    [Fact]
    public async Task GetModuleFromEdgeAgentTwin_ModuleIdInOtherCase_ReturnsTheModule()
    {
        // Act
        var actual = await sut.GetModuleFromEdgeAgentTwin(EdgeAgentTwin, "SIMULATION");

        // Assert
        actual.Should().NotBeNull();
        actual!.Name.Should().Be("simulation");
    }

    [Fact]
    public async Task GetModuleFromEdgeAgentTwin_ModuleNotOnTheDevice_ReturnsNull()
    {
        // Act
        var actual = await sut.GetModuleFromEdgeAgentTwin(EdgeAgentTwin, "unknown");

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetModuleFromEdgeAgentTwin_EdgeAgentHasNotReportedYet_ReturnsNull()
    {
        // Arrange
        var twin = new Twin { Properties = new TwinProperties { Reported = new TwinCollection("{}") } };

        // Act
        var actual = await sut.GetModuleFromEdgeAgentTwin(twin, "simulation");

        // Assert
        actual.Should().BeNull();
    }
}