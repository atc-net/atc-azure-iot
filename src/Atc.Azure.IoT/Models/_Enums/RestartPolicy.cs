// ReSharper disable CheckNamespace
namespace Atc.Azure.IoT.Models;

/// <summary>
/// When edgeAgent restarts a module. In a deployment manifest the values are written as
/// <c>always</c>, <c>never</c>, <c>on-failure</c> and <c>on-unhealthy</c>
/// (see <see cref="Extensions.RestartPolicyExtensions.ToManifestValue"/>).
/// </summary>
public enum RestartPolicy
{
    /// <summary>Not set. It can't be written into a deployment manifest.</summary>
    Unknown,

    /// <summary>Always restart the module when it stops.</summary>
    Always,

    /// <summary>Never restart the module.</summary>
    Never,

    /// <summary>Restart the module when it crashes.</summary>
    OnFailure,

    /// <summary>Restart the module when it crashes or reports itself as unhealthy.</summary>
    OnUnhealthy,
}