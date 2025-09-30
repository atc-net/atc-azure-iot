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
    /// Command used to restart a specific module within the Edge Agent.
    /// </summary>
    public const string DirectMethodRestartModule = "RestartModule";

    /// <summary>
    /// Command that triggers IoT Edge module logs upload.
    /// Collects module logs, packages them into a ZIP archive,
    /// and uploads the bundle to a designated Azure Blob Storage container.
    /// </summary>
    public const string DirectMethodUploadModuleLogs = "UploadModuleLogs";

    /// <summary>
    /// Command that triggers an IoT Edge support bundle upload.
    /// Collects module logs, packages them into a ZIP archive,
    /// and uploads the bundle to a designated Azure Blob Storage container.
    /// </summary>
    public const string DirectMethodUploadSupportBundle = "UploadSupportBundle";

    /// <summary>
    /// Command used to retrieve the status of an existing request.
    /// </summary>
    public const string DirectMethodGetTaskStatus = "GetTaskStatus";
}