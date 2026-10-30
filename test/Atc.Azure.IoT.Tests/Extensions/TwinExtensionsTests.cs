namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class TwinExtensionsTests
{
    [Fact]
    public void GetDesiredProperties_ReadsTheDesiredPropertiesAsTheGivenType()
    {
        // Arrange
        var twin = new Twin { Properties = new TwinProperties { Desired = new TwinCollection("""{ "interval": 5, "unit": "seconds" }""") } };

        // Act
        var actual = twin.GetDesiredProperties<SampleSettings>();

        // Assert
        actual.Should().Be(new SampleSettings(5, "seconds"));
    }

    [Fact]
    public void GetReportedProperties_EdgeAgentTwin_ReadsTheModulesWithTheirNames()
    {
        // Arrange
        var twin = new Twin
        {
            Properties = new TwinProperties
            {
                Reported = new TwinCollection(
                    """
                    {
                      "systemModules": { "edgeHub": {{Module}} },
                      "modules": { "simulation": {{Module}} }
                    }
                    """.Replace("{{Module}}", RunningModule, StringComparison.Ordinal)),
            },
        };

        // Act
        var actual = twin.GetReportedProperties<EdgeAgentReportedProperties>();

        // Assert
        actual.SystemModules!.Select(x => x.Name).Should().Equal("edgeHub");
        actual.Modules!.Select(x => x.Name).Should().Equal("simulation");
        actual.Modules![0].RuntimeStatus.Should().Be("running");
    }

    [Fact]
    public void GetReportedProperties_TimeNeverSet_IsReadAsNull()
    {
        // Arrange
        // edgeAgent reports a time that was never set as the minimum date.
        var twin = new Twin
        {
            Properties = new TwinProperties
            {
                Reported = new TwinCollection(
                    """{ "modules": { "simulation": {{Module}} } }""".Replace(
                        "{{Module}}",
                        RunningModule.Replace("\"lastExitTimeUtc\": null", "\"lastExitTimeUtc\": \"0001-01-01T00:00:00Z\"", StringComparison.Ordinal),
                        StringComparison.Ordinal)),
            },
        };

        // Act
        var actual = twin.GetReportedProperties<EdgeAgentReportedProperties>();

        // Assert
        actual.Modules![0].LastExitTimeUtc.Should().BeNull();
    }

    private const string RunningModule =
        """
        {
          "exitCode": 0,
          "statusDescription": "running",
          "runtimeStatus": "running",
          "lastExitTimeUtc": null,
          "lastStartTimeUtc": "2026-10-30T12:00:00Z",
          "restartCount": 0,
          "settings": { "image": "simulation:1.0" }
        }
        """;

    internal sealed record SampleSettings(
        int Interval,
        string Unit);
}