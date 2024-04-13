namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// Provides services for managing device enrollments within the Azure Device Provisioning Service (DPS),
/// including creating, retrieving, and deleting individual device enrollments, as well as bulk retrieval of enrollments.
/// This service supports operations for both TPM and non-TPM devices, facilitating device provisioning and management
/// through DPS with functions to handle individual enrollment processes, manage device enrollment records, and perform
/// enrollment operations such as creating TPM-based enrollments with specific properties and tags.
/// </summary>
public sealed partial class DeviceProvisioningService : DeviceProvisioningServiceBase, IDeviceProvisioningService, IDisposable
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    private ProvisioningServiceClient? client;

    public DeviceProvisioningService(
        ILoggerFactory loggerFactory,
        DeviceProvisioningServiceOptions options)
    {
        logger = loggerFactory.CreateLogger<DeviceProvisioningService>();
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        ValidateAndAssign(options.ConnectionString, Assign);
    }

    /// <summary>
    /// Retrieves a specific individual enrollment from Azure DPS using the provided registration ID.
    /// This method returns null if the enrollment does not exist.
    /// </summary>
    /// <param name="registrationId">The registration ID of the individual enrollment to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The IndividualEnrollment instance if found, or null if not found.</returns>
    public async Task<IndividualEnrollment?> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(registrationId);

        try
        {
            var enrollment = await client!.GetIndividualEnrollmentAsync(
                registrationId,
                cancellationToken);

            return enrollment;
        }
        catch (ProvisioningServiceClientHttpException ex) when (ex.ErrorMessage.Equals("Not Found", StringComparison.OrdinalIgnoreCase) &&
                                                                ex.Message.Contains('{', StringComparison.Ordinal))
        {
            var json = ex.Message[ex.Message.IndexOf('{', StringComparison.Ordinal)..];
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, jsonSerializerOptions);

            LogIndividualEnrollmentNotFound(
                registrationId,
                errorResponse?.ErrorCode ?? null,
                errorResponse?.Message ?? ex.GetLastInnerMessage());

            return null;
        }
        catch (Exception ex)
        {
            LogIndividualEnrollmentNotFound(
                registrationId,
                errorCode: null,
                ex.GetLastInnerMessage());

            return null;
        }
    }

    // TODO: Extend with optional parameter to e.g. limit to specific type of individual enrollment

    /// <summary>
    /// Retrieves all individual enrollments registered in Azure DPS.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of all IndividualEnrollments.</returns>
    public async Task<IEnumerable<IndividualEnrollment>> GetIndividualEnrollments(
        CancellationToken cancellationToken)
    {
        var enrollments = new List<IndividualEnrollment>();
        var querySpecification = new QuerySpecification("SELECT * FROM enrollments");

        using var query = client!.CreateIndividualEnrollmentQuery(
            querySpecification,
            cancellationToken);

        while (query.HasNext())
        {
            var page = await query.NextAsync();
            foreach (var item in page.Items)
            {
                if (item is IndividualEnrollment individualEnrollment)
                {
                    enrollments.Add(individualEnrollment);
                }
            }
        }

        return enrollments;
    }

    /// <summary>
    /// Creates or updates an individual TPM enrollment in Azure DPS with the provided parameters.
    /// Returns the enrollment result along with any error messages if the operation fails.
    /// </summary>
    /// <param name="endorsementKey">The TPM endorsement key.</param>
    /// <param name="registrationId">The registration ID for the enrollment.</param>
    /// <param name="serialNumber">The serial number of the device.</param>
    /// <param name="tags">Optional. The tags to be applied to the device twin.</param>
    /// <param name="desiredProperties">Optional. The desired properties to be applied to the device twin.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>Tuple containing the IndividualEnrollment instance and an error message if applicable.</returns>
    public async Task<(IndividualEnrollment? Enrollment, string? ErrorMessage)> CreateIndividualTpmEnrollment(
        string endorsementKey,
        string registrationId,
        string serialNumber,
        Dictionary<string, string>? tags,
        Dictionary<string, string>? desiredProperties,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(endorsementKey);
        ArgumentException.ThrowIfNullOrEmpty(registrationId);
        ArgumentException.ThrowIfNullOrEmpty(serialNumber);

        try
        {
            var individualEnrollment = BuildIndividualEnrollment(
                endorsementKey,
                registrationId,
                serialNumber,
                tags,
                desiredProperties);

            var individualEnrollmentResponse = await client!.CreateOrUpdateIndividualEnrollmentAsync(
                individualEnrollment,
                cancellationToken);

            return (individualEnrollmentResponse, null);
        }
        catch (ProvisioningServiceClientHttpException ex) when (ex.ErrorMessage.Equals("Bad Request", StringComparison.OrdinalIgnoreCase) &&
                                                                ex.Message.Contains('{', StringComparison.Ordinal))
        {
            var json = ex.Message[ex.Message.IndexOf('{', StringComparison.Ordinal)..];
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, jsonSerializerOptions);

            LogIndividualTpmEnrollmentBadRequest(
                registrationId,
                errorResponse?.ErrorCode ?? null,
                errorResponse?.Message ?? ex.GetLastInnerMessage());

            return (null, errorResponse?.Message ?? ex.GetLastInnerMessage());
        }
        catch (ProvisioningServiceClientHttpException ex) when (ex.ErrorMessage.Equals("Conflict", StringComparison.OrdinalIgnoreCase) &&
                                                                ex.Message.Contains('{', StringComparison.Ordinal))
        {
            var json = ex.Message[ex.Message.IndexOf('{', StringComparison.Ordinal)..];
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, jsonSerializerOptions);

            LogIndividualTpmEnrollmentConflict(
                registrationId,
                errorResponse?.ErrorCode ?? null,
                errorResponse?.Message ?? ex.GetLastInnerMessage());

            return (null, errorResponse?.Message ?? ex.GetLastInnerMessage());
        }
        catch (Exception ex)
        {
            LogIndividualTpmEnrollmentFailed(
                registrationId,
                ex.GetLastInnerMessage());

            return (null, ex.GetLastInnerMessage());
        }
    }

    /// <summary>
    /// Deletes an individual enrollment from Azure DPS using the provided registration ID.
    /// </summary>
    /// <param name="registrationId">The registration ID of the individual enrollment to delete.</param>
    /// <returns>Indicates whether the operation succeeded.</returns>
    public async Task<bool> DeleteIndividualEnrollment(
        string registrationId)
    {
        ArgumentException.ThrowIfNullOrEmpty(registrationId);

        try
        {
            await client!.DeleteIndividualEnrollmentAsync(registrationId);

            return true;
        }
        catch (ProvisioningServiceClientHttpException ex) when (ex.ErrorMessage.Equals("Not Found", StringComparison.OrdinalIgnoreCase) &&
                                                                ex.Message.Contains('{', StringComparison.Ordinal))
        {
            var json = ex.Message[ex.Message.IndexOf('{', StringComparison.Ordinal)..];
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, jsonSerializerOptions);

            LogDeleteIndividualEnrollmentNotFound(
                registrationId,
                errorResponse?.ErrorCode ?? null,
                errorResponse?.Message ?? ex.GetLastInnerMessage());

            return false;
        }
        catch (Exception ex)
        {
            LogDeleteIndividualEnrollmentNotFound(
                registrationId,
                errorCode: null,
                ex.GetLastInnerMessage());

            return false;
        }
    }

    private static IndividualEnrollment BuildIndividualEnrollment(
        string endorsementKey,
        string registrationId,
        string serialNumber,
        Dictionary<string, string>? tags,
        Dictionary<string, string>? desiredProperties)
    {
        var attestation = new TpmAttestation(endorsementKey);
        var individualEnrollment = new IndividualEnrollment(
            registrationId,
            attestation)
        {
            DeviceId = serialNumber,
            ProvisioningStatus = ProvisioningStatus.Enabled,
            Capabilities = new DeviceCapabilities
            {
                IotEdge = true,
            },
            ReprovisionPolicy = new ReprovisionPolicy
            {
                MigrateDeviceData = true,
            },
        };

        var twinState = new TwinState(
            tags.ToTwinCollection(),
            desiredProperties.ToTwinCollection());

        individualEnrollment.InitialTwinState = twinState;
        return individualEnrollment;
    }

    protected override void Assign(
        string connectionString)
    {
        client = ProvisioningServiceClient.CreateFromConnectionString(connectionString);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(
        bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (client is null)
        {
            return;
        }

        client.Dispose();
        client = null;
    }
}