namespace Atc.Azure.IoT.Tests.Extensions;

public sealed class IoTHubResilienceExtensionsTests
{
    [Fact]
    public async Task AddIoTHubRequestOptions_ErrorKeepsHappening_RetriesAsManyTimesAsTheOptionsSay()
    {
        // Arrange
        var attempts = 0;
        var pipeline = new ResiliencePipelineBuilder<int>()
            .AddIoTHubRequestOptions(
                new IoTHubRequestOptions { MaxRetryAttempts = 2, Delay = TimeSpan.Zero },
                shouldHandle: _ => ValueTask.FromResult(true),
                onRetry: null)
            .Build();

        // Act
        var act = async () => await pipeline.ExecuteAsync<int>(
            _ =>
            {
                attempts++;
                throw new IotHubException("IoT Hub is busy", isTransient: true);
            },
            TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<IotHubException>();
        attempts.Should().Be(3);
    }

    [Fact]
    public async Task AddIoTHubRequestOptions_ErrorIsNotHandled_DoesNotRetry()
    {
        // Arrange
        var attempts = 0;
        var pipeline = new ResiliencePipelineBuilder<int>()
            .AddIoTHubRequestOptions(
                new IoTHubRequestOptions { MaxRetryAttempts = 2, Delay = TimeSpan.Zero },
                shouldHandle: _ => ValueTask.FromResult(false),
                onRetry: null)
            .Build();

        // Act
        var act = async () => await pipeline.ExecuteAsync<int>(
            _ =>
            {
                attempts++;
                throw new UnauthorizedException("The SAS token has an invalid signature");
            },
            TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        attempts.Should().Be(1);
    }

    [Fact]
    public async Task AddIoTHubRequestOptions_EachRetry_CallsOnRetryWithTheAttemptNumber()
    {
        // Arrange
        var retriedAttempts = new List<int>();
        var pipeline = new ResiliencePipelineBuilder<int>()
            .AddIoTHubRequestOptions(
                new IoTHubRequestOptions { MaxRetryAttempts = 2, Delay = TimeSpan.Zero },
                shouldHandle: _ => ValueTask.FromResult(true),
                onRetry: args =>
                {
                    retriedAttempts.Add(args.AttemptNumber);
                    return ValueTask.CompletedTask;
                })
            .Build();

        // Act
        var act = async () => await pipeline.ExecuteAsync<int>(
            _ => throw new IotHubException("IoT Hub is busy", isTransient: true),
            TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<IotHubException>();
        retriedAttempts.Should().Equal(0, 1);
    }
}