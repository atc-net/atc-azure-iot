namespace Atc.Azure.IoT;

/// <summary>
/// The EdgeHubConstants class contains constant string values
/// which are used within the context of the Iot Edge Hub System Module.
/// </summary>
public static class EdgeHubConstants
{
    /// <summary>
    /// The ModuleId constant represents the unique identifier for the Edge Hub module.
    /// </summary>
    /// <remarks>
    /// The value "$edgeHub" is a special reserved keyword used internally to refer to the Iot Edge Hub itself.
    /// </remarks>
    public const string ModuleId = "$edgeHub";
}