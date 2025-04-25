namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// DeviceProvisioningManager LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class DeviceProvisioningService
{
    private readonly ILogger<DeviceProvisioningService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.IndividualTpmEnrollmentFailed,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to create individual TPM enrollment for registrationId '{RegistrationId}': {ErrorMessage}")]
    private partial void LogIndividualTpmEnrollmentFailed(
        string registrationId,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.IndividualTpmEnrollmentBadRequest,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to create individual TPM enrollment for registrationId '{RegistrationId}'. Error: {ErrorCode} # {ErrorMessage}")]
    private partial void LogIndividualTpmEnrollmentBadRequest(
        string registrationId,
        int? errorCode,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.IndividualTpmEnrollmentConflict,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to create individual TPM enrollment for registrationId '{RegistrationId}'. Error: {ErrorCode} # {ErrorMessage}")]
    private partial void LogIndividualTpmEnrollmentConflict(
        string registrationId,
        int? errorCode,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.IndividualEnrollmentNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Individual enrollment with registrationId '{RegistrationId}' not found. Error: {ErrorCode} # {ErrorMessage}")]
    private partial void LogIndividualEnrollmentNotFound(
        string registrationId,
        int? errorCode,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.DeleteIndividualEnrollmentNotFound,
        Level = LogLevel.Error,
        Message = "{CallerMethodName}({CallerLineNumber}) - Failed to delete individual enrollment with registrationId '{RegistrationId}'. Error: {ErrorCode} # {ErrorMessage}")]
    private partial void LogDeleteIndividualEnrollmentNotFound(
        string registrationId,
        int? errorCode,
        string? errorMessage,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}