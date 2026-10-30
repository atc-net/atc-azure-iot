namespace Atc.Azure.IoT.Extensions;

/// <summary>
/// Reports the caller's cancellation as <see cref="OperationCanceledException"/>.
/// </summary>
/// <remarks>
/// The IoT Hub and DPS SDKs catch the <see cref="TaskCanceledException"/> of a cancelled HTTP request and throw their
/// own exception instead (<c>IotHubException</c> or <c>ProvisioningServiceClientException</c>), so callers can't tell a
/// cancelled call from a failed one.
/// </remarks>
internal static class CancellationTaskExtensions
{
    /// <summary>
    /// Awaits the task, and when it fails while <paramref name="cancellationToken"/> is cancelled, throws an
    /// <see cref="OperationCanceledException"/> with the SDK's exception as <see cref="Exception.InnerException"/>.
    /// </summary>
    /// <typeparam name="T">The type of the task's result.</typeparam>
    /// <param name="task">The SDK call.</param>
    /// <param name="cancellationToken">The caller's token, as passed to the SDK call.</param>
    /// <returns>The task's result.</returns>
    public static async Task<T> NormalizeCancellation<T>(
        this Task<T> task,
        CancellationToken cancellationToken)
    {
        try
        {
            return await task;
        }
        catch (Exception ex) when (IsCallerCancellation(ex, cancellationToken))
        {
            throw new OperationCanceledException(ex.Message, ex, cancellationToken);
        }
    }

    /// <inheritdoc cref="NormalizeCancellation{T}(Task{T}, CancellationToken)"/>
    public static async Task NormalizeCancellation(
        this Task task,
        CancellationToken cancellationToken)
    {
        try
        {
            await task;
        }
        catch (Exception ex) when (IsCallerCancellation(ex, cancellationToken))
        {
            throw new OperationCanceledException(ex.Message, ex, cancellationToken);
        }
    }

    private static bool IsCallerCancellation(
        Exception exception,
        CancellationToken cancellationToken)
        => cancellationToken.IsCancellationRequested &&
           exception is not OperationCanceledException;
}