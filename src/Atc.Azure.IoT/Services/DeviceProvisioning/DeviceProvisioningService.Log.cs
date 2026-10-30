namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// DeviceProvisioningManager LoggerMessages.
/// </summary>
public sealed partial class DeviceProvisioningService
{
    private readonly ILogger<DeviceProvisioningService> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.IndividualEnrollmentNotFound,
        Level = LogLevel.Warning,
        Message = "{CallerMethodName}({CallerLineNumber}) - Individual enrollment with registrationId '{RegistrationId}' not found")]
    private partial void LogIndividualEnrollmentNotFound(
        string registrationId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.DeviceProvisioningManager.DeleteIndividualEnrollmentNotFound,
        Level = LogLevel.Information,
        Message = "{CallerMethodName}({CallerLineNumber}) - Individual enrollment with registrationId '{RegistrationId}' was not deleted, because it doesn't exist")]
    private partial void LogIndividualEnrollmentNotFoundOnDelete(
        string registrationId,
        [CallerMemberName] string callerMethodName = "",
        [CallerLineNumber] int callerLineNumber = 0);
}