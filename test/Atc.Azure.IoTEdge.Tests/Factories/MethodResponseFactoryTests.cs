namespace Atc.Azure.IoTEdge.Tests.Factories;

public sealed class MethodResponseFactoryTests
{
    [Fact]
    public void Create_ReturnsTheStatusCode()
        => new MethodResponseFactory()
            .Create(HttpStatusCode.NotFound, "The endpoint doesn't exist")
            .Status.Should().Be(404);

    [Fact]
    public void Create_TextData_ReturnsItAsAJsonString()
        => new MethodResponseFactory()
            .Create(HttpStatusCode.OK, "Done")
            .ResultAsJson.Should().Be("\"Done\"");

    [Fact]
    public void Create_ObjectData_WritesItsPropertiesInCamelCase()
    {
        // Act
        var actual = new MethodResponseFactory().Create(HttpStatusCode.OK, new SampleResult("endpoint-1", 3));

        // Assert
        using var document = JsonDocument.Parse(actual.ResultAsJson);
        document.RootElement.EnumerateObject().Select(x => x.Name).Should().Equal("endpointId", "nodeCount");
    }

    [Fact]
    public void Create_ObjectData_WritesCompactJsonToSaveResponseSpace()
        => new MethodResponseFactory()
            .Create(HttpStatusCode.OK, new SampleResult("endpoint-1", 3))
            .ResultAsJson.Should().Be("""{"endpointId":"endpoint-1","nodeCount":3}""");

    [Fact]
    public void Create_SerializerOptionsGiven_UsesThem()
        => new MethodResponseFactory(new JsonSerializerOptions())
            .Create(HttpStatusCode.OK, new SampleResult("endpoint-1", 3))
            .ResultAsJson.Should().Be("""{"EndpointId":"endpoint-1","NodeCount":3}""");

    internal sealed record SampleResult(
        string EndpointId,
        int NodeCount);
}