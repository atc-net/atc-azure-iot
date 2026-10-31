namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class IotHubExceptionExtensionsTests
{
    // As the IoT Hub SDK words a wrong key: IoT Hub's JSON inside the SDK's JSON.
    private const string SdkErrorMessage =
        """
        {"Message":"{\"errorCode\":401002,\"message\":\"The specified SAS token has an invalid signature. It does not match either the primary or secondary key.\",\"trackingId\":\"44532333438F4D849692E2475BCC34D2-G2\",\"timestampUtc\":\"2026-10-31T08:47:26.546070131Z\",\"info\":null}","ExceptionMessage":""}
        """;

    [Fact]
    public void GetErrorMessage_SdkWrapsIoTHubsJson_ReturnsIoTHubsMessageAndErrorCode()
        => new UnauthorizedException(SdkErrorMessage)
            .GetErrorMessage()
            .Should().Be("The specified SAS token has an invalid signature. It does not match either the primary or secondary key. (401002)");

    [Fact]
    public void GetErrorMessage_IoTHubsJsonWithoutTheSdkWrapper_ReturnsIoTHubsMessageAndErrorCode()
        => new IotHubException("""{"errorCode":404001,"message":"Device not found."}""")
            .GetErrorMessage()
            .Should().Be("Device not found. (404001)");

    [Fact]
    public void GetErrorMessage_JsonMessageWithoutAnErrorCode_ReturnsOnlyTheMessage()
        => new IotHubException("""{"Message":"The operation timed out.","ExceptionMessage":""}""")
            .GetErrorMessage()
            .Should().Be("The operation timed out.");

    [Theory]
    [InlineData("The device is offline")]
    [InlineData("ErrorCode:DeviceNotOnline;Timed out waiting for device")]
    [InlineData("{ not json")]
    [InlineData("""{"other":"value"}""")]
    public void GetErrorMessage_MessageNotInIoTHubsFormat_ReturnsItUnchanged(string message)
        => new IotHubException(message)
            .GetErrorMessage()
            .Should().Be(message);

    [Fact]
    public void ToJsonPayload_SdkWrapsIoTHubsJson_UsesIoTHubsMessage()
    {
        // Act
        var actual = new UnauthorizedException(SdkErrorMessage).ToJsonPayload();

        // Assert
        using var document = JsonDocument.Parse(actual);
        document.RootElement.GetProperty("message").GetString().Should()
            .Be("The specified SAS token has an invalid signature. It does not match either the primary or secondary key. (401002)");
    }

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