namespace Atc.Azure.IoT.Extensions;

public static class IoTHubResilienceExtensions
{
    public static ResiliencePipelineBuilder<TResult> AddIoTHubRequestOptions<TResult>(
        this ResiliencePipelineBuilder<TResult> builder,
        IoTHubRequestOptions requestOptions,
        Func<RetryPredicateArguments<TResult>, ValueTask<bool>> shouldHandle,
        Func<OnRetryArguments<TResult>, ValueTask>? onRetry)
        => builder.AddRetry(new RetryStrategyOptions<TResult>
        {
            MaxRetryAttempts = requestOptions.MaxRetryAttempts,
            BackoffType = requestOptions.BackoffType,
            Delay = requestOptions.Delay,
            MaxDelay = requestOptions.MaxDelay,
            ShouldHandle = shouldHandle,
            OnRetry = onRetry,
        });
}