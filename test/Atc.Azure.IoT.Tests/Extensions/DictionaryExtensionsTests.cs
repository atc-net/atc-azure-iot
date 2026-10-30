namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class DictionaryExtensionsTests
{
    [Fact]
    public void ToTwinCollection_NoDictionary_ReturnsNull()
        => ((Dictionary<string, string>?)null).ToTwinCollection().Should().BeNull();

    [Fact]
    public void ToTwinCollection_EmptyDictionary_ReturnsNull()
        => new Dictionary<string, string>(StringComparer.Ordinal).ToTwinCollection().Should().BeNull();

    [Fact]
    public void ToTwinCollection_PlainKeys_BecomeTopLevelProperties()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["location"] = "factory-1",
            ["line"] = "3",
        };

        // Act
        var actual = dictionary.ToTwinCollection();

        // Assert
        actual.Should().NotBeNull();
        ((string)actual!["location"]).Should().Be("factory-1");
        ((string)actual["line"]).Should().Be("3");
    }

    [Fact]
    public void ToTwinCollection_KeysWithADot_BecomeNestedProperties()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Location.Site"] = "factory-1",
            ["Location.Line"] = "3",
        };

        // Act
        var actual = dictionary.ToTwinCollection();

        // Assert
        actual.Should().NotBeNull();
        ((string)actual!["Location"]["Site"]).Should().Be("factory-1");
        ((string)actual["Location"]["Line"]).Should().Be("3");
    }
}