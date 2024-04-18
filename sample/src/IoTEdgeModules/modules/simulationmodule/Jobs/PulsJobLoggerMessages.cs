namespace SimulationModule.Jobs;

/// <summary>
/// PulsJob LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class PulsJob
{
    private readonly ILogger<PulsJob> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.JobStarted,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - '{jobName}' started.")]
    private partial void LogJobStarted(
        string jobName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.JobEnded,
        Level = LogLevel.Information,
        Message = "{callerMethodName}({callerLineNumber}) - '{jobName}' ended.")]
    private partial void LogJobEnded(
        string jobName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = Atc.Azure.IoTEdge.LoggingEventIdConstants.FailedToAcquireModuleClient,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Failed to acquire ModuleClient for the job '{jobName}'.")]
    private partial void LogFailedToAcquireModuleClient(
        string jobName,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.UnhandledExceptionInJob,
        Level = LogLevel.Error,
        Message = "{callerMethodName}({callerLineNumber}) - Unhandled exception occurred in job '{jobName}' - '{errorMessage}'.")]
    private partial void LogUnhandledExceptionInJob(
        string jobName,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}