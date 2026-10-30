namespace Atc.Azure.IoT.Tests.Providers;

public sealed class ConfigurationContentProviderTests
{
    private const string DeploymentManifest =
        """
        {
          "modulesContent": {
            "$edgeAgent": {
              "properties.desired": {
                "schemaVersion": "1.1",
                "modules": {
                  "simulation": {
                    "settings": { "image": "simulation:1.0", "createOptions": "{}" }
                  }
                }
              }
            },
            "$edgeHub": {
              "properties.desired": {
                "schemaVersion": "1.1",
                "routes": { "upstream": "FROM /messages/* INTO $upstream" }
              }
            }
          }
        }
        """;

    private readonly ConfigurationContentProvider sut = new();

    [Fact]
    public async Task GetConfigurationContent_ValidManifestStream_ReturnsTheDesiredPropertiesOfEachSystemModule()
    {
        // Arrange
        using var stream = CreateStream(DeploymentManifest);

        // Act
        var actual = await sut.GetConfigurationContent(stream, TestContext.Current.CancellationToken);

        // Assert
        actual.ModulesContent.Keys.Should().BeEquivalentTo("$edgeAgent", "$edgeHub");
        actual.ModulesContent["$edgeHub"].Should().ContainKey("properties.desired");
    }

    [Fact]
    public async Task GetConfigurationContent_ValidManifestStream_LeavesTheCallersStreamOpen()
    {
        // Arrange
        using var stream = CreateStream(DeploymentManifest);

        // Act
        await sut.GetConfigurationContent(stream, TestContext.Current.CancellationToken);

        // Assert
        stream.CanRead.Should().BeTrue();
    }

    [Theory]
    [InlineData("this is not json")]
    [InlineData("[1, 2, 3]")]
    [InlineData("""{ "modulesContent": """)]
    public Task GetConfigurationContent_StreamIsNotAJsonObject_ThrowsInvalidDataException(string content)
    {
        // Arrange
        var stream = CreateStream(content);

        // Act
        var act = () => sut.GetConfigurationContent(stream, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<InvalidDataException>().WithMessage("The deployment manifest is not a valid JSON object*");
    }

    [Fact]
    public Task GetConfigurationContent_StreamIsEmpty_ThrowsInvalidDataException()
    {
        // Arrange
        var stream = CreateStream(string.Empty);

        // Act
        var act = () => sut.GetConfigurationContent(stream, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<InvalidDataException>().WithMessage("The deployment manifest is empty.");
    }

    [Fact]
    public async Task GetConfigurationContent_CallerCancels_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        using var stream = CreateStream(DeploymentManifest);

        // Act
        var act = () => sut.GetConfigurationContent(stream, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetConfigurationContent_ValidManifestFile_ReturnsItsConfigurationContent()
    {
        // Arrange
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"atc-azure-iot-{Guid.NewGuid():N}.json"));
        await File.WriteAllTextAsync(file.FullName, DeploymentManifest, TestContext.Current.CancellationToken);

        try
        {
            // Act
            var actual = await sut.GetConfigurationContent(file, TestContext.Current.CancellationToken);

            // Assert
            actual.ModulesContent.Keys.Should().BeEquivalentTo("$edgeAgent", "$edgeHub");
        }
        finally
        {
            file.Delete();
        }
    }

    [Fact]
    public Task GetConfigurationContent_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        // Arrange
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"atc-azure-iot-{Guid.NewGuid():N}.json"));

        // Act
        var act = () => sut.GetConfigurationContent(file, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<FileNotFoundException>();
    }

    private static MemoryStream CreateStream(string content)
        => new(Encoding.UTF8.GetBytes(content));
}