namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Docker;

/// <summary>
/// DockerService LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public partial class DockerService
{
    private readonly ILogger<DockerService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerClientCreated,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully created docker client.")]
    private partial void LogDockerClientCreated(
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadStarted,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Started downloading docker image '{imageName}'.")]
    private partial void LogDockerImageDownloadStarted(
        string imageName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully downloaded docker image '{imageName}'.")]
    private partial void LogDockerImageDownloadSucceeded(
        string imageName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadStatus,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - -> {message}")]
    private partial void LogDockerImageDownloadStatus(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to download docker image '{imageName}': '{errorMessage}'.")]
    private partial void LogDockerImageDownloadFailed(
        string imageName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationStarted,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Started creating container '{containerName}'.")]
    private partial void LogContainerCreationStarted(
        string containerName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully created container '{containerName}'.")]
    private partial void LogContainerCreationSucceeded(
        string containerName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to create container '{containerName}': '{errorMessage}'.")]
    private partial void LogContainerCreationFailed(
        string containerName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStarting,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Starting container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStarting(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStartSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully started container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStartSucceeded(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStartFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to start container '{containerName}' with containerId '{containerId}': '{errorMessage}'.")]
    private partial void LogContainerStartFailed(
        string containerName,
        string containerId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopMissingContainerId,
        Level = LogLevel.Warning,
        Message = "{callerMethodName}({callerLineNumber}) - Can not stop container '{containerName}' due to missing containerId.")]
    private partial void LogContainerStopMissingContainerId(
        string containerName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopping,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Stopping container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStopping(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully stopped container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStopSucceeded(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerRemovalStarting,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Removing container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerRemovalStarting(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerRemovalSucceeded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Successfully removed container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerRemovalSucceeded(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopOrRemovalFailed,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to stop/remove container '{containerName}' with containerId '{containerId}': '{errorMessage}'.")]
    private partial void LogContainerStopOrRemovalFailed(
        string containerName,
        string containerId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorCheck,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Starting check if IoTEdge module '{moduleName}' is running in container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogIotEdgeEmulatorCheck(
        string moduleName,
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorWaiting,
        Level = LogLevel.Trace,
        Message = "{callerMethodName}({callerLineNumber}) - Waiting {secondsBetweenRetries} seconds for IoTEdge module '{moduleName}' to become available in container '{containerName}' with containerId '{containerId}' - retry ({currentIteration}/{numberOfRetries}).")]
    private partial void LogIotEdgeEmulatorWaiting(
        string moduleName,
        string containerName,
        string containerId,
        int secondsBetweenRetries,
        int currentIteration,
        int numberOfRetries,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorReady,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - IoTEdge module '{moduleName}' is now running in container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogIotEdgeEmulatorReady(
        string moduleName,
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorFetchingVariables,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - Fetching IoT Edge environment variables from module '{moduleName}'.")]
    private partial void LogIotEdgeEmulatorFetchingVariables(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerProcessCommandFailure,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failure running docker command - error: '{message}'.")]
    private partial void LogDockerProcessCommandFailure(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}