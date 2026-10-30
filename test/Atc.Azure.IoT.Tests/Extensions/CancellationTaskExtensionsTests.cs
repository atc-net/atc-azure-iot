namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class CancellationTaskExtensionsTests
{
    [Fact]
    public async Task NormalizeCancellation_CallSucceeds_ReturnsItsResult()
    {
        // Act
        var actual = await Task.FromResult(42).NormalizeCancellation(TestContext.Current.CancellationToken);

        // Assert
        actual.Should().Be(42);
    }

    [Fact]
    public async Task NormalizeCancellation_CallFailsAfterTheCallerCancelled_ThrowsOperationCanceledExceptionWithTheSdkErrorInside()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var sdkError = new IotHubException("A task was canceled.", new TaskCanceledException());

        // Act
        var act = () => Task.FromException<int>(sdkError).NormalizeCancellation(cancellationTokenSource.Token);

        // Assert
        var thrown = await act.Should().ThrowAsync<OperationCanceledException>();
        thrown.Which.InnerException.Should().BeSameAs(sdkError);
        thrown.Which.CancellationToken.Should().Be(cancellationTokenSource.Token);
    }

    [Fact]
    public Task NormalizeCancellation_CallFailsWithoutCancellation_ThrowsTheOriginalError()
    {
        // Act
        var act = () => Task.FromException<int>(new IotHubException("IoT Hub is busy")).NormalizeCancellation(TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowExactlyAsync<IotHubException>();
    }

    [Fact]
    public async Task NormalizeCancellation_CallThrowsOperationCanceledException_ThrowsItUnchanged()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var cancellation = new OperationCanceledException(cancellationTokenSource.Token);

        // Act
        var act = () => Task.FromException<int>(cancellation).NormalizeCancellation(cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.Should().BeSameAs(cancellation);
    }

    [Fact]
    public async Task NormalizeCancellation_CallWithoutResultFailsAfterTheCallerCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var sdkError = new IotHubException("A task was canceled.", new TaskCanceledException());

        // Act
        var act = () => Task.FromException(sdkError).NormalizeCancellation(cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeSameAs(sdkError);
    }
}