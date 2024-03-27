namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// The main DeviceProvisioningService - Handles call execution.
/// </summary>
public sealed partial class DeviceProvisioningService : IDeviceProvisioningService
{
    private readonly ProvisioningServiceClient client;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public DeviceProvisioningService(
        ILogger<DeviceProvisioningService> logger,
        ProvisioningServiceClient client)
    {
        this.logger = logger;
        this.client = client;
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
    }

    public async Task<IndividualEnrollment?> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(registrationId);

        try
        {
            var enrollment = await client
                .GetIndividualEnrollmentAsync(
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

    public async Task<IEnumerable<IndividualEnrollment>> GetAllIndividualEnrollments(
        CancellationToken cancellationToken)
    {
        var enrollments = new List<IndividualEnrollment>();
        var querySpecification = new QuerySpecification("SELECT * FROM enrollments");

        using var query = client
            .CreateIndividualEnrollmentQuery(
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

            var individualEnrollmentResponse = await client
                .CreateOrUpdateIndividualEnrollmentAsync(
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

    public async Task<bool> DeleteIndividualEnrollment(
        string registrationId)
    {
        ArgumentException.ThrowIfNullOrEmpty(registrationId);

        try
        {
            await client
                .DeleteIndividualEnrollmentAsync(registrationId);

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
}