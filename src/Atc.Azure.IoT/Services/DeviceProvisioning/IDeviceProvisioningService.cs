namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// Provides services for managing device enrollments within the Azure Device Provisioning Service (DPS),
/// including creating, retrieving, and deleting individual device enrollments, as well as bulk retrieval of enrollments.
/// This service supports operations for both TPM and non-TPM devices, facilitating device provisioning and management
/// through DPS with functions to handle individual enrollment processes, manage device enrollment records, and perform
/// enrollment operations such as creating TPM-based enrollments with specific properties and tags.
/// </summary>
/// <remarks>
/// <see cref="GetIndividualEnrollment"/> returns <see langword="null"/>, and <see cref="DeleteIndividualEnrollment"/>
/// returns <see langword="false"/>, only when the enrollment doesn't exist. Every other failure is thrown as the DPS
/// SDK's exception, usually a <see cref="ProvisioningServiceClientHttpException"/> whose <c>StatusCode</c> and
/// <c>Body</c> say what DPS rejected. A cancelled <see cref="CancellationToken"/> throws
/// <see cref="OperationCanceledException"/>; the DPS SDK reports a cancelled request as a
/// <see cref="ProvisioningServiceClientException"/>, which becomes the <see cref="Exception.InnerException"/>.
/// </remarks>
public interface IDeviceProvisioningService
{
    /// <summary>
    /// Retrieves a specific individual enrollment from Azure DPS using the provided registration ID.
    /// </summary>
    /// <param name="registrationId">The registration ID of the individual enrollment to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The individual enrollment, or <see langword="null"/> if it doesn't exist.</returns>
    Task<IndividualEnrollment?> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all individual enrollments registered in Azure DPS.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>All individual enrollments; empty if there are none.</returns>
    Task<IReadOnlyCollection<IndividualEnrollment>> GetIndividualEnrollments(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates an individual TPM enrollment for an IoT Edge device in Azure DPS.
    /// </summary>
    /// <param name="endorsementKey">The TPM endorsement key.</param>
    /// <param name="registrationId">The registration ID for the enrollment.</param>
    /// <param name="deviceId">The id of the device.</param>
    /// <param name="tags">Optional. The tags to be applied to the device twin.</param>
    /// <param name="desiredProperties">Optional. The desired properties to be applied to the device twin.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The created or updated individual enrollment.</returns>
    /// <exception cref="ProvisioningServiceClientHttpException">
    /// DPS rejected the enrollment, for example 400 for an invalid endorsement key or 409 for a conflict; its
    /// <c>Body</c> holds DPS's error message.
    /// </exception>
    Task<IndividualEnrollment> CreateIndividualTpmEnrollment(
        string endorsementKey,
        string registrationId,
        string deviceId,
        Dictionary<string, string>? tags,
        Dictionary<string, string>? desiredProperties,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an individual enrollment from Azure DPS using the provided registration ID.
    /// </summary>
    /// <param name="registrationId">The registration ID of the individual enrollment to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// <see langword="true"/> if the enrollment was deleted; <see langword="false"/> if it doesn't exist
    /// (for example because it was already deleted).
    /// </returns>
    Task<bool> DeleteIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken = default);
}