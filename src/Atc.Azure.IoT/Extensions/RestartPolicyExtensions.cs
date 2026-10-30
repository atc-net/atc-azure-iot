namespace Atc.Azure.IoT.Extensions;

/// <summary>
/// Converts <see cref="RestartPolicy"/> to and from the values a deployment manifest uses.
/// </summary>
public static class RestartPolicyExtensions
{
    private const string Always = "always";
    private const string Never = "never";
    private const string OnFailure = "on-failure";
    private const string OnUnhealthy = "on-unhealthy";

    /// <summary>
    /// Gets the value a deployment manifest uses for the restart policy, for example <c>on-failure</c>.
    /// </summary>
    /// <param name="restartPolicy">The restart policy.</param>
    /// <returns>The deployment manifest value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The restart policy is <see cref="RestartPolicy.Unknown"/>.</exception>
    public static string ToManifestValue(this RestartPolicy restartPolicy)
        => restartPolicy switch
        {
            RestartPolicy.Always => Always,
            RestartPolicy.Never => Never,
            RestartPolicy.OnFailure => OnFailure,
            RestartPolicy.OnUnhealthy => OnUnhealthy,
            _ => throw new ArgumentOutOfRangeException(
                nameof(restartPolicy),
                restartPolicy,
                $"A deployment manifest needs a restart policy: {Always}, {Never}, {OnFailure} or {OnUnhealthy}."),
        };

    /// <summary>
    /// Reads a restart policy from a deployment manifest value, for example <c>on-failure</c>. The case is ignored.
    /// </summary>
    /// <param name="value">The deployment manifest value.</param>
    /// <returns>The restart policy.</returns>
    /// <exception cref="ArgumentException">The value is not a restart policy IoT Edge knows.</exception>
    public static RestartPolicy FromManifestValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.ToLowerInvariant() switch
        {
            Always => RestartPolicy.Always,
            Never => RestartPolicy.Never,
            OnFailure => RestartPolicy.OnFailure,
            OnUnhealthy => RestartPolicy.OnUnhealthy,
            _ => throw new ArgumentException(
                $"'{value}' is not a restart policy. Use {Always}, {Never}, {OnFailure} or {OnUnhealthy}.",
                nameof(value)),
        };
    }
}