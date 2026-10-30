namespace Atc.Azure.IoT.Extensions;

/// <summary>
/// Converts <see cref="ModuleStatus"/> to and from the values a deployment manifest uses.
/// </summary>
public static class ModuleStatusExtensions
{
    private const string Running = "running";
    private const string Stopped = "stopped";

    /// <summary>
    /// Gets the value a deployment manifest uses for the desired module status: <c>running</c> or <c>stopped</c>.
    /// </summary>
    /// <param name="moduleStatus">The desired module status.</param>
    /// <returns>The deployment manifest value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The status is <see cref="ModuleStatus.Unknown"/>.</exception>
    public static string ToManifestValue(this ModuleStatus moduleStatus)
        => moduleStatus switch
        {
            ModuleStatus.Running => Running,
            ModuleStatus.Stopped => Stopped,
            _ => throw new ArgumentOutOfRangeException(
                nameof(moduleStatus),
                moduleStatus,
                $"A deployment manifest needs a module status: {Running} or {Stopped}."),
        };

    /// <summary>
    /// Reads a desired module status from a deployment manifest value: <c>running</c> or <c>stopped</c>. The case is ignored.
    /// </summary>
    /// <param name="value">The deployment manifest value.</param>
    /// <returns>The desired module status.</returns>
    /// <exception cref="ArgumentException">The value is not a module status IoT Edge knows.</exception>
    public static ModuleStatus FromManifestValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.ToLowerInvariant() switch
        {
            Running => ModuleStatus.Running,
            Stopped => ModuleStatus.Stopped,
            _ => throw new ArgumentException(
                $"'{value}' is not a module status. Use {Running} or {Stopped}.",
                nameof(value)),
        };
    }
}