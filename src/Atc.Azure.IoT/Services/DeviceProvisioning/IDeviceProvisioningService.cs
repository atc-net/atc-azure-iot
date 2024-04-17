namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// Provides services for managing device enrollments within the Azure Device Provisioning Service (DPS),
/// including creating, retrieving, and deleting individual device enrollments, as well as bulk retrieval of enrollments.
/// This service supports operations for both TPM and non-TPM devices, facilitating device provisioning and management
/// through DPS with functions to handle individual enrollment processes, manage device enrollment records, and perform
/// enrollment operations such as creating TPM-based enrollments with specific properties and tags.
/// </summary>
public interface IDeviceProvisioningService
{
    /// <summary>
    /// Retrieves a specific individual enrollment from Azure DPS using the provided registration ID.
    /// This method returns null if the enrollment does not exist.
    /// </summary>
    /// <param name="registrationId">The registration ID of the individual enrollment to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The IndividualEnrollment instance if found, or null if not found.</returns>
    Task<IndividualEnrollment?> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all individual enrollments registered in Azure DPS.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of all IndividualEnrollments.</returns>
    Task<IEnumerable<IndividualEnrollment>> GetIndividualEnrollments(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates an individual TPM enrollment in Azure DPS with the provided parameters.
    /// Returns the enrollment result along with any error messages if the operation fails.
    /// </summary>
    /// <param name="endorsementKey">The TPM endorsement key.</param>
    /// <param name="registrationId">The registration ID for the enrollment.</param>
    /// <param name="deviceId">The id of the device.</param>
    /// <param name="tags">Optional. The tags to be applied to the device twin.</param>
    /// <param name="desiredProperties">Optional. The desired properties to be applied to the device twin.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>Tuple containing the IndividualEnrollment instance and an error message if applicable.</returns>
    Task<(IndividualEnrollment? Enrollment, string? ErrorMessage)> CreateIndividualTpmEnrollment(
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
    /// <returns>Indicates whether the operation succeeded.</returns>
    Task<bool> DeleteIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken = default);
}