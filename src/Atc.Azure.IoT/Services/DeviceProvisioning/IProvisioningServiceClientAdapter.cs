namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// The calls <see cref="DeviceProvisioningService"/> makes to the DPS service client. The SDK's
/// <see cref="ProvisioningServiceClient"/> has a private constructor and non-virtual methods, so this interface lets
/// tests replace it.
/// </summary>
internal interface IProvisioningServiceClientAdapter : IDisposable
{
    Task<IndividualEnrollment> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IndividualEnrollment>> GetIndividualEnrollments(
        CancellationToken cancellationToken);

    Task<IndividualEnrollment> CreateOrUpdateIndividualEnrollment(
        IndividualEnrollment individualEnrollment,
        CancellationToken cancellationToken);

    Task DeleteIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken);
}