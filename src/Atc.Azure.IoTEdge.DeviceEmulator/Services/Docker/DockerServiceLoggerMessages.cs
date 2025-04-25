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
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully created docker client.")]
    private partial void LogDockerClientCreated(
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadStarted,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Started downloading docker image '{ImageName}'.")]
    private partial void LogDockerImageDownloadStarted(
        string imageName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadSucceeded,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully downloaded docker image '{ImageName}'.")]
    private partial void LogDockerImageDownloadSucceeded(
        string imageName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadStatus,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - -> {Message}")]
    private partial void LogDockerImageDownloadStatus(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerImageDownloadFailed,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to download docker image '{ImageName}': '{ErrorMessage}'.")]
    private partial void LogDockerImageDownloadFailed(
        string imageName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationStarted,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Started creating container '{ContainerName}'.")]
    private partial void LogContainerCreationStarted(
        string containerName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationSucceeded,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully created container '{ContainerName}'.")]
    private partial void LogContainerCreationSucceeded(
        string containerName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerCreationFailed,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to create container '{ContainerName}': '{errorMessage}'.")]
    private partial void LogContainerCreationFailed(
        string containerName,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStarting,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogContainerStarting(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStartSucceeded,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully started container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogContainerStartSucceeded(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStartFailed,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to start container '{ContainerName}' with containerId '{ContainerId}': '{ErrorMessage}'.")]
    private partial void LogContainerStartFailed(
        string containerName,
        string containerId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopMissingContainerId,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Can not stop container '{ContainerName}' due to missing ContainerId.")]
    private partial void LogContainerStopMissingContainerId(
        string containerName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopping,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Stopping container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogContainerStopping(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopSucceeded,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully stopped container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogContainerStopSucceeded(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerRemovalStarting,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Removing container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogContainerRemovalStarting(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerRemovalSucceeded,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Successfully removed container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogContainerRemovalSucceeded(
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerContainerStopOrRemovalFailed,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to stop/remove container '{ContainerName}' with containerId '{ContainerId}': '{ErrorMessage}'.")]
    private partial void LogContainerStopOrRemovalFailed(
        string containerName,
        string containerId,
        string errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorCheck,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Starting check if IoTEdge module '{ModuleName}' is running in container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogIotEdgeEmulatorCheck(
        string moduleName,
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorWaiting,
        Level = LogLevel.Trace,
        Message = "{CallerMethodName}({CallerLineNumber}) - Waiting {SecondsBetweenRetries} seconds for IoTEdge module '{ModuleName}' to become available in container '{ContainerName}' with containerId '{ContainerId}' - retry ({CurrentIteration}/{NumberOfRetries}).")]
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
        Message = "{CallerMethodName}({CallerLineNumber}) - IoTEdge module '{ModuleName}' is now running in container '{ContainerName}' with containerId '{ContainerId}'.")]
    private partial void LogIotEdgeEmulatorReady(
        string moduleName,
        string containerName,
        string containerId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.IotEdgeEmulatorFetchingVariables,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Fetching IoT Edge environment variables from module '{ModuleName}'.")]
    private partial void LogIotEdgeEmulatorFetchingVariables(
        string moduleName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DockerProcessCommandFailure,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failure running docker command - error: '{Message}'.")]
    private partial void LogDockerProcessCommandFailure(
        string message,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}