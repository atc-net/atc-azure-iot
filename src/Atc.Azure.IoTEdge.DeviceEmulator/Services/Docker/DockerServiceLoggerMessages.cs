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
        Message = "Successfully created docker client.")]
    private partial void LogDockerClientCreated();

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadStarted,
        Level = LogLevel.Information,
        Message = "Started downloading docker image '{imageName}'.")]
    private partial void LogDockerImageDownloadStarted(string imageName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadSucceeded,
        Level = LogLevel.Information,
        Message = "Successfully downloaded docker image '{imageName}'.")]
    private partial void LogDockerImageDownloadSucceeded(string imageName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadStatus,
        Level = LogLevel.Trace,
        Message = "-> {message}")]
    private partial void LogDockerImageDownloadStatus(string message);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadFailed,
        Level = LogLevel.Error,
        Message = "Failed to download docker image '{imageName}': '{errorMessage}'.")]
    private partial void LogDockerImageDownloadFailed(string imageName, string errorMessage);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationStarted,
        Level = LogLevel.Information,
        Message = "Started creating container '{containerName}'.")]
    private partial void LogContainerCreationStarted(string containerName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationSucceeded,
        Level = LogLevel.Information,
        Message = "Successfully created container '{containerName}'.")]
    private partial void LogContainerCreationSucceeded(string containerName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationFailed,
        Level = LogLevel.Error,
        Message = "Failed to create container '{containerName}': '{errorMessage}'.")]
    private partial void LogContainerCreationFailed(string containerName, string errorMessage);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStarting,
        Level = LogLevel.Information,
        Message = "Starting container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStarting(string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStartSucceeded,
        Level = LogLevel.Information,
        Message = "Successfully started container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStartSucceeded(string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStartFailed,
        Level = LogLevel.Error,
        Message = "Failed to start container '{containerName}' with containerId '{containerId}': '{errorMessage}'.")]
    private partial void LogContainerStartFailed(string containerName, string containerId, string errorMessage);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopMissingContainerId,
        Level = LogLevel.Warning,
        Message = "Can not stop container '{containerName}' due to missing containerId.")]
    private partial void LogContainerStopMissingContainerId(string containerName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopping,
        Level = LogLevel.Information,
        Message = "Stopping container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStopping(string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopSucceeded,
        Level = LogLevel.Information,
        Message = "Successfully stopped container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerStopSucceeded(string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerRemovalStarting,
        Level = LogLevel.Information,
        Message = "Removing container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerRemovalStarting(string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerRemovalSucceeded,
        Level = LogLevel.Information,
        Message = "Successfully removed container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogContainerRemovalSucceeded(string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopOrRemovalFailed,
        Level = LogLevel.Error,
        Message = "Failed to stop/remove container '{containerName}' with containerId '{containerId}': '{errorMessage}'.")]
    private partial void LogContainerStopOrRemovalFailed(string containerName, string containerId, string errorMessage);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorCheck,
        Level = LogLevel.Information,
        Message = "Starting check if IoTEdge module '{moduleName}' is running in container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogIotEdgeEmulatorCheck(string moduleName, string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorWaiting,
        Level = LogLevel.Trace,
        Message = "Waiting {secondsBetweenRetries} seconds for IoTEdge module '{moduleName}' to become available in container '{containerName}' with containerId '{containerId}' - retry ({currentIteration}/{numberOfRetries}).")]
    private partial void LogIotEdgeEmulatorWaiting(
        string moduleName,
        string containerName,
        string containerId,
        int secondsBetweenRetries,
        int currentIteration,
        int numberOfRetries);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorReady,
        Level = LogLevel.Information,
        Message = "IoTEdge module '{moduleName}' is now running in container '{containerName}' with containerId '{containerId}'.")]
    private partial void LogIotEdgeEmulatorReady(string moduleName, string containerName, string containerId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorFetchingVariables,
        Level = LogLevel.Information,
        Message = "Fetching IoT Edge environment variables from module '{moduleName}'.")]
    private partial void LogIotEdgeEmulatorFetchingVariables(string moduleName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerProcessCommandFailure,
        Level = LogLevel.Error,
        Message = "Failure running docker command - error: '{message}'.")]
    private partial void LogDockerProcessCommandFailure(string message);
}