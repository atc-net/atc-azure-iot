namespace Atc.Azure.IoT.Tests.Serialization.JsonConverters;

public sealed class JsonEdgeHubRouteConverterTests
{
    [Fact]
    public void Read_RouteAsText_ReturnsTheRouteWithoutPriorityOrTimeToLive()
        => JsonSerializer.Deserialize<EdgeHubRoute>("\"FROM /messages/* INTO $upstream\"")
            .Should().Be(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null));

    [Fact]
    public void Read_RouteObject_ReturnsTheRouteWithItsPriorityAndTimeToLive()
        => JsonSerializer.Deserialize<EdgeHubRoute>("""{ "route": "FROM /messages/* INTO $upstream", "priority": 0, "timeToLiveSecs": 86400 }""")
            .Should().Be(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: 0, TimeToLiveSecs: 86400));

    [Fact]
    public void Read_RouteObjectWithOnlyTheRoute_LeavesPriorityAndTimeToLiveEmpty()
        => JsonSerializer.Deserialize<EdgeHubRoute>("""{ "route": "FROM /messages/* INTO $upstream" }""")
            .Should().Be(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null));

    [Fact]
    public void Read_RouteIsANumber_ThrowsJsonException()
    {
        // Act
        var act = () => JsonSerializer.Deserialize<EdgeHubRoute>("42");

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Write_RouteWithoutPriorityOrTimeToLive_WritesTheRouteText()
        => JsonSerializer.Serialize(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: null, TimeToLiveSecs: null))
            .Should().Be("\"FROM /messages/* INTO $upstream\"");

    [Fact]
    public void Write_RouteWithPriority_WritesTheRouteObject()
        => JsonSerializer.Serialize(new EdgeHubRoute("FROM /messages/* INTO $upstream", Priority: 1, TimeToLiveSecs: null))
            .Should().Be("""{"route":"FROM /messages/* INTO $upstream","priority":1}""");
}