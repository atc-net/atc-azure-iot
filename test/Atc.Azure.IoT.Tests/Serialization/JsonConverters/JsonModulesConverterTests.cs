namespace Atc.Azure.IoT.Tests.Serialization.JsonConverters;

public sealed class JsonModulesConverterTests
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public JsonModulesConverterTests()
    {
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create(
            new JsonSerializerFactorySettings
            {
                UseConverterDatetimeOffsetMinToNull = true,
            });

        jsonSerializerOptions.Converters.Add(new JsonModulesConverter());
    }

    [Fact]
    public void ShouldDeserialize_WhenStartupOrder_IsNumber()
    {
        // Arrange
        const string json = """
        {
          "dummyModule": {
            "exitCode": 0,
            "statusDescription": "running",
            "runtimeStatus": "running",
            "lastExitTimeUtc": null,
            "lastStartTimeUtc": "2025-06-25T12:00:00Z",
            "lastRestartTimeUtc": null,
            "restartCount": 0,
            "startupOrder": 1,
            "settings": {},
            "environment": null
          }
        }
        """;

        // Act
        var modules = JsonSerializer.Deserialize<List<Module>>(json, jsonSerializerOptions);

        // Assert
        Assert.NotNull(modules);
        Assert.Single(modules);
        Assert.Equal(1, modules![0].StartupOrder);
    }

    [Fact]
    public void ShouldDeserialize_WhenStartupOrder_IsString()
    {
        // Arrange
        const string json = """
        {
          "dummyModule": {
            "exitCode": 0,
            "statusDescription": "running",
            "runtimeStatus": "running",
            "lastExitTimeUtc": null,
            "lastStartTimeUtc": "2025-06-25T12:00:00Z",
            "lastRestartTimeUtc": null,
            "restartCount": 0,
            "startupOrder": "1",
            "settings": {},
            "environment": null
          }
        }
        """;

        // Act
        var modules = JsonSerializer.Deserialize<List<Module>>(json, jsonSerializerOptions);

        // Assert
        Assert.NotNull(modules);
        Assert.Single(modules);
        Assert.Equal(1, modules![0].StartupOrder);
    }

    [Fact]
    public void ShouldDeserialize_WhenStartupOrder_IsMissing()
    {
        // Arrange
        const string json = """
                            {
                              "dummyModule": {
                                "exitCode": 0,
                                "statusDescription": "running",
                                "runtimeStatus": "running",
                                "lastExitTimeUtc": null,
                                "lastStartTimeUtc": "2025-06-25T12:00:00Z",
                                "lastRestartTimeUtc": null,
                                "restartCount": 0,
                                "settings": {},
                                "environment": null
                              }
                            }
                            """;

        // Act
        var modules = JsonSerializer.Deserialize<List<Module>>(json, jsonSerializerOptions);

        // Assert
        Assert.NotNull(modules);
        Assert.Single(modules);
        Assert.Null(modules![0].StartupOrder);
    }
}