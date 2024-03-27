namespace Atc.Azure.IoT;

/// <summary>
/// The EdgeAgentConstants class contains constant string values
/// which are used within the context of the Iot Edge Agent System Module.
/// </summary>
public static class EdgeAgentConstants
{
    /// <summary>
    /// The ModuleId constant represents the unique identifier for the Edge Agent module.
    /// </summary>
    /// <remarks>
    /// The value "$edgeAgent" is a special reserved keyword used internally to refer to the Iot Edge Agent itself.
    /// </remarks>
    public const string ModuleId = "$edgeAgent";

    /// <summary>
    /// The DirectMethodRestartModule constant represents the command used to restart a specific module within the Edge Agent.
    /// </summary>
    public const string DirectMethodRestartModule = "RestartModule";
}