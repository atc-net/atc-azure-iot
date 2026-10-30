namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class IotHubExceptionExtensionsTests
{
    [Fact]
    public void ToJsonPayload_ReturnsTheMessageAndTheErrorCode()
    {
        // Arrange
        var exception = new DeviceNotFoundException(ErrorCode.DeviceNotOnline, "The device is offline");

        // Act
        var actual = exception.ToJsonPayload();

        // Assert
        actual.Should().Be("""{"message":"The device is offline","errorCode":"DeviceNotOnline"}""");
    }

    [Fact]
    public void ToJsonPayload_MessageWithQuotesAndBackslashes_IsStillValidJson()
    {
        // Arrange
        const string message = """ErrorCode:DeviceNotOnline;{"Message":"Timed out waiting for device","Path":"C:\\logs"}""";
        var exception = new IotHubException(message);

        // Act
        var actual = exception.ToJsonPayload();

        // Assert
        using var document = JsonDocument.Parse(actual);
        document.RootElement.GetProperty("message").GetString().Should().Be(message);
    }

    [Fact]
    public void ToJsonPayload_InnerException_UsesTheInnermostMessage()
    {
        // Arrange
        var exception = new IotHubException("Outer", new InvalidOperationException("The innermost reason"));

        // Act
        var actual = exception.ToJsonPayload();

        // Assert
        using var document = JsonDocument.Parse(actual);
        document.RootElement.GetProperty("message").GetString().Should().Be("The innermost reason");
    }
}