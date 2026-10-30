namespace Atc.Azure.IoT.Tests;

/// <summary>
/// Simulates an SDK call that the caller cancels: the SDK notices the cancelled request and throws its own exception
/// instead of an <see cref="OperationCanceledException"/>.
/// </summary>
internal static class SdkCancellation
{
    public static async Task<T> CancelThenThrow<T>(
        CancellationTokenSource cancellationTokenSource,
        Exception sdkError)
    {
        await cancellationTokenSource.CancelAsync();
        throw sdkError;
    }

    public static async Task CancelThenThrow(
        CancellationTokenSource cancellationTokenSource,
        Exception sdkError)
    {
        await cancellationTokenSource.CancelAsync();
        throw sdkError;
    }
}