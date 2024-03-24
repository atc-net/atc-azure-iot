namespace Atc.Azure.IoT.Services.DeviceProvisioning;

public interface IDeviceProvisioningService
{
    Task<IndividualEnrollment?> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken);

    Task<IEnumerable<IndividualEnrollment>> GetAllIndividualEnrollments(
        CancellationToken cancellationToken);

    Task<(IndividualEnrollment? Enrollment, string? ErrorMessage)> CreateIndividualTpmEnrollment(
        string endorsementKey,
        string registrationId,
        string serialNumber,
        Dictionary<string, string>? tags,
        Dictionary<string, string>? desiredProperties,
        CancellationToken cancellationToken);

    Task<bool> DeleteIndividualEnrollment(
        string registrationId);
}