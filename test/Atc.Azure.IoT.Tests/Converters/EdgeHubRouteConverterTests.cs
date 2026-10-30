namespace Atc.Azure.IoT.Tests.Converters;

public sealed class EdgeHubRouteConverterTests
{
    [Fact]
    public void ReadJson_RouteAsText_ReturnsTheRouteWithoutPriorityOrTimeToLive()
    {
        // Act
        var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<EdgeHubRoute>("\"FROM /messages/* INTO $upstream\"");

        // Assert
        actual.Should().Be(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null));
    }

    [Fact]
    public void ReadJson_RouteObject_ReturnsTheRouteWithItsPriorityAndTimeToLive()
    {
        // Act
        var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<EdgeHubRoute>(
            """{ "route": "FROM /messages/* INTO $upstream", "priority": 0, "timeToLiveSecs": 86400 }""");

        // Assert
        actual.Should().Be(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: 0, TimeToLiveSecs: 86400));
    }

    [Fact]
    public void ReadJson_RouteObjectWithOnlyTheRoute_LeavesPriorityAndTimeToLiveEmpty()
    {
        // Act
        var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<EdgeHubRoute>("""{ "route": "FROM /messages/* INTO $upstream" }""");

        // Assert
        actual.Should().Be(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null));
    }

    [Fact]
    public void ReadJson_RoutesOfADeploymentManifest_ReadsBothFormats()
    {
        // Act
        var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, EdgeHubRoute>>(
            """
            {
              "upstream": "FROM /messages/* INTO $upstream",
              "alarms": { "route": "FROM /messages/modules/alarms/* INTO $upstream", "priority": 0 }
            }
            """);

        // Assert
        actual.Should().NotBeNull();
        actual!["upstream"].Priority.Should().BeNull();
        actual["alarms"].Priority.Should().Be(0);
    }
}