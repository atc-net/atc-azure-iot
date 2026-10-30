namespace Atc.Azure.IoT.Services.DeviceProvisioning;

/// <summary>
/// Passes the calls of <see cref="DeviceProvisioningService"/> on to the SDK's <see cref="ProvisioningServiceClient"/>.
/// </summary>
internal sealed class ProvisioningServiceClientAdapter : IProvisioningServiceClientAdapter
{
    private const string AllEnrollmentsQuery = "SELECT * FROM enrollments";

    private readonly ProvisioningServiceClient client;

    public ProvisioningServiceClientAdapter(
        string connectionString)
        => client = ProvisioningServiceClient.CreateFromConnectionString(connectionString);

    public Task<IndividualEnrollment> GetIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken)
        => client.GetIndividualEnrollmentAsync(
            registrationId,
            cancellationToken);

    public async Task<IReadOnlyCollection<IndividualEnrollment>> GetIndividualEnrollments(
        CancellationToken cancellationToken)
    {
        var enrollments = new List<IndividualEnrollment>();

        using var query = client.CreateIndividualEnrollmentQuery(
            new QuerySpecification(AllEnrollmentsQuery),
            cancellationToken);

        while (query.HasNext())
        {
            var page = await query.NextAsync();
            enrollments.AddRange(page.Items.OfType<IndividualEnrollment>());
        }

        return enrollments.AsReadOnly();
    }

    public Task<IndividualEnrollment> CreateOrUpdateIndividualEnrollment(
        IndividualEnrollment individualEnrollment,
        CancellationToken cancellationToken)
        => client.CreateOrUpdateIndividualEnrollmentAsync(
            individualEnrollment,
            cancellationToken);

    public Task DeleteIndividualEnrollment(
        string registrationId,
        CancellationToken cancellationToken)
        => client.DeleteIndividualEnrollmentAsync(
            registrationId,
            cancellationToken);

    public void Dispose()
        => client.Dispose();
}