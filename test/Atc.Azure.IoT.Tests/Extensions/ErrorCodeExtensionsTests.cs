namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class ErrorCodeExtensionsTests
{
    [Theory]
    [InlineData(ErrorCode.InvalidProtocolVersion, 400)]
    [InlineData(ErrorCode.InvalidOperation, 400)]
    [InlineData(ErrorCode.ArgumentInvalid, 400)]
    [InlineData(ErrorCode.ArgumentNull, 400)]
    [InlineData(ErrorCode.IotHubFormatError, 400)]
    [InlineData(ErrorCode.DeviceDefinedMultipleTimes, 400)]
    [InlineData(ErrorCode.BulkRegistryOperationFailure, 400)]
    [InlineData(ErrorCode.IotHubUnauthorizedAccess, 401)]
    [InlineData(ErrorCode.IotHubQuotaExceeded, 403)]
    [InlineData(ErrorCode.DeviceMaximumQueueDepthExceeded, 403)]
    [InlineData(ErrorCode.DeviceNotFound, 404)]
    [InlineData(ErrorCode.ModuleNotFound, 404)]
    [InlineData(ErrorCode.DeviceNotOnline, 404)]
    [InlineData(ErrorCode.DeviceAlreadyExists, 409)]
    [InlineData(ErrorCode.ModuleAlreadyExistsOnDevice, 409)]
    [InlineData(ErrorCode.PreconditionFailed, 412)]
    [InlineData(ErrorCode.MessageTooLarge, 413)]
    [InlineData(ErrorCode.TooManyDevices, 413)]
    [InlineData(ErrorCode.GenericTooManyRequests, 429)]
    [InlineData(ErrorCode.ThrottlingException, 429)]
    [InlineData(ErrorCode.ThrottleBacklogLimitExceeded, 429)]
    [InlineData(ErrorCode.ThrottlingBacklogTimeout, 429)]
    [InlineData(ErrorCode.ThrottlingMaxActiveJobCountExceeded, 429)]
    [InlineData(ErrorCode.DeviceThrottlingLimitExceeded, 429)]
    [InlineData(ErrorCode.ServerError, 500)]
    [InlineData(ErrorCode.ServiceUnavailable, 503)]
    [InlineData(ErrorCode.InvalidErrorCode, 500)]
    public void ShouldReturnFirstThreeDigitsOfErrorCode(ErrorCode errorCode, int expected)
    {
        // Act
        var actual = errorCode.ToHttpStatusCode();

        // Assert
        Assert.Equal(expected, actual);
    }
}