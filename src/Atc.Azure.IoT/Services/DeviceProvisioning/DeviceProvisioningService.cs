namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// Provides services for managing device enrollments within the Azure Device Provisioning Service (DPS),
/// including creating, retrieving, and deleting individual device enrollments, as well as bulk retrieval of enrollments.
/// This service supports operations for both TPM and non-TPM devices, facilitating device provisioning and management
/// through DPS with functions to handle individual enrollment processes, manage device enrollment records, and perform
/// enrollment operations such as creating TPM-based enrollments with specific properties and tags.
/// </summary>
public sealed partial class DeviceProvisioningService : ServiceBase, IDeviceProvisioningService, IDisposable
{
    private IProvisioningServiceClientAdapter? client;

    public DeviceProvisioningService(
        DeviceProvisioningServiceOptions options,
        ILoggerFactory? loggerFactory = null)
    {
        ValidateAndAssign(options.ConnectionString, Assign);
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<DeviceProvisioningService>()
            : NullLogger<DeviceProvisioningService>.Instance;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceProvisioningService"/> class with a given DPS client, for tests.
    /// </summary>
    /// <param name="client">The DPS client to use.</param>
    /// <param name="loggerFactory">The logger factory, or <see langword="null"/> for no logging.</param>
    internal DeviceProvisioningService(
        IProvisioningServiceClientAdapter client,
        ILoggerFactory? loggerFactory = null)
    {
        this.client = client;
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<DeviceProvisioningService>()
            : NullLogger<DeviceProvisioningService>.Instance;
    }

    /// <inheritdoc />
    public async Task<IndividualEnrollment?> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(registrationId);

        try
        {
            return await client!.GetIndividualEnrollment(
                    registrationId,
                    cancellationToken)
                .NormalizeCancellation(cancellationToken);
        }
        catch (ProvisioningServiceClientHttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            LogIndividualEnrollmentNotFound(registrationId);

            return null;
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<IndividualEnrollment>> GetIndividualEnrollments(
        CancellationToken cancellationToken = default)
        => client!.GetIndividualEnrollments(cancellationToken)
            .NormalizeCancellation(cancellationToken);

    /// <inheritdoc />
    public Task<IndividualEnrollment> CreateIndividualTpmEnrollment(
        string endorsementKey,
        string registrationId,
        string deviceId,
        Dictionary<string, string>? tags,
        Dictionary<string, string>? desiredProperties,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(endorsementKey);
        ArgumentException.ThrowIfNullOrEmpty(registrationId);
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        var individualEnrollment = BuildIndividualEnrollment(
            endorsementKey,
            registrationId,
            deviceId,
            tags,
            desiredProperties);

        return client!.CreateOrUpdateIndividualEnrollment(
                individualEnrollment,
                cancellationToken)
            .NormalizeCancellation(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(registrationId);

        try
        {
            await client!.DeleteIndividualEnrollment(
                    registrationId,
                    cancellationToken)
                .NormalizeCancellation(cancellationToken);
        }
        catch (ProvisioningServiceClientHttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            LogIndividualEnrollmentNotFoundOnDelete(registrationId);

            return false;
        }

        return true;
    }

    private static IndividualEnrollment BuildIndividualEnrollment(
        string endorsementKey,
        string registrationId,
        string deviceId,
        Dictionary<string, string>? tags,
        Dictionary<string, string>? desiredProperties)
    {
        var attestation = new TpmAttestation(endorsementKey);
        var individualEnrollment = new IndividualEnrollment(
            registrationId,
            attestation)
        {
            DeviceId = deviceId,
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
        client = new ProvisioningServiceClientAdapter(connectionString);
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