namespace Atc.Azure.IoT.Tests.Services.DeviceProvisioning;

public sealed class DeviceProvisioningServiceTests : IDisposable
{
    private const string RegistrationId = "registration-1";
    private const string DeviceId = "device-1";

    private static readonly string EndorsementKey = Convert.ToBase64String(new byte[32]);

    private readonly IProvisioningServiceClientAdapter client = Substitute.For<IProvisioningServiceClientAdapter>();
    private readonly DeviceProvisioningService sut;

    public DeviceProvisioningServiceTests()
        => sut = new DeviceProvisioningService(client);

    public void Dispose()
    {
        sut.Dispose();
        client.Dispose();
    }

    [Fact]
    public async Task GetIndividualEnrollment_EnrollmentExists_ReturnsIt()
    {
        // Arrange
        var enrollment = CreateEnrollment();

        client
            .GetIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .Returns(enrollment);

        // Act
        var actual = await sut.GetIndividualEnrollment(RegistrationId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(enrollment);
    }

    [Fact]
    public async Task GetIndividualEnrollment_EnrollmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        client
            .GetIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .ThrowsAsync(CreateDpsError(HttpStatusCode.NotFound));

        // Act
        var actual = await sut.GetIndividualEnrollment(RegistrationId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetIndividualEnrollment_DpsRejectsTheKey_ThrowsTheDpsError()
    {
        // Arrange
        client
            .GetIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .ThrowsAsync(CreateDpsError(HttpStatusCode.Unauthorized));

        // Act
        var act = () => sut.GetIndividualEnrollment(RegistrationId, TestContext.Current.CancellationToken);

        // Assert
        (await act.Should().ThrowAsync<ProvisioningServiceClientHttpException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetIndividualEnrollment_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheDpsError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The DPS SDK reports the caller's cancellation as a ProvisioningServiceClientException.
        client
            .GetIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow<IndividualEnrollment>(
                cancellationTokenSource,
                new ProvisioningServiceClientException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.GetIndividualEnrollment(RegistrationId, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<ProvisioningServiceClientException>();
    }

    [Fact]
    public async Task GetIndividualEnrollments_ReturnsAllEnrollments()
    {
        // Arrange
        IReadOnlyCollection<IndividualEnrollment> enrollments = [CreateEnrollment(), CreateEnrollment()];

        client
            .GetIndividualEnrollments(Arg.Any<CancellationToken>())
            .Returns(enrollments);

        // Act
        var actual = await sut.GetIndividualEnrollments(TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(enrollments);
    }

    [Fact]
    public async Task GetIndividualEnrollments_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheDpsError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The DPS SDK reports the caller's cancellation as a ProvisioningServiceClientException.
        client
            .GetIndividualEnrollments(Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow<IReadOnlyCollection<IndividualEnrollment>>(
                cancellationTokenSource,
                new ProvisioningServiceClientException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.GetIndividualEnrollments(cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<ProvisioningServiceClientException>();
    }

    [Fact]
    public async Task CreateIndividualTpmEnrollment_SendsAnEnabledIoTEdgeEnrollmentWithTheTpmKey()
    {
        // Arrange
        IndividualEnrollment? sent = null;
        var created = CreateEnrollment();

        client
            .CreateOrUpdateIndividualEnrollment(Arg.Do<IndividualEnrollment>(x => sent = x), Arg.Any<CancellationToken>())
            .Returns(created);

        // Act
        var actual = await sut.CreateIndividualTpmEnrollment(EndorsementKey, RegistrationId, DeviceId, tags: null, desiredProperties: null, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(created);
        sent.Should().NotBeNull();
        sent!.RegistrationId.Should().Be(RegistrationId);
        sent.DeviceId.Should().Be(DeviceId);
        sent.ProvisioningStatus.Should().Be(ProvisioningStatus.Enabled);
        sent.Capabilities.IotEdge.Should().BeTrue();
        sent.ReprovisionPolicy.MigrateDeviceData.Should().BeTrue();
        sent.Attestation.Should().BeOfType<TpmAttestation>()
            .Which.EndorsementKey.Should().Be(EndorsementKey);
    }

    [Fact]
    public async Task CreateIndividualTpmEnrollment_TagsAndDesiredPropertiesGiven_PutsThemInTheDevicesInitialTwin()
    {
        // Arrange
        IndividualEnrollment? sent = null;

        client
            .CreateOrUpdateIndividualEnrollment(Arg.Do<IndividualEnrollment>(x => sent = x), Arg.Any<CancellationToken>())
            .Returns(CreateEnrollment());

        var tags = new Dictionary<string, string>(StringComparer.Ordinal) { ["location"] = "factory-1" };
        var desiredProperties = new Dictionary<string, string>(StringComparer.Ordinal) { ["interval"] = "5" };

        // Act
        await sut.CreateIndividualTpmEnrollment(EndorsementKey, RegistrationId, DeviceId, tags, desiredProperties, TestContext.Current.CancellationToken);

        // Assert
        sent.Should().NotBeNull();
        ((string)sent!.InitialTwinState.Tags["location"]).Should().Be("factory-1");
        ((string)sent.InitialTwinState.DesiredProperties["interval"]).Should().Be("5");
    }

    [Fact]
    public async Task CreateIndividualTpmEnrollment_DpsRejectsTheEnrollment_ThrowsTheDpsError()
    {
        // Arrange
        client
            .CreateOrUpdateIndividualEnrollment(Arg.Any<IndividualEnrollment>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(CreateDpsError(HttpStatusCode.BadRequest));

        // Act
        var act = () => sut.CreateIndividualTpmEnrollment(EndorsementKey, RegistrationId, DeviceId, tags: null, desiredProperties: null, TestContext.Current.CancellationToken);

        // Assert
        (await act.Should().ThrowAsync<ProvisioningServiceClientHttpException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateIndividualTpmEnrollment_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheDpsError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The DPS SDK reports the caller's cancellation as a ProvisioningServiceClientException.
        client
            .CreateOrUpdateIndividualEnrollment(Arg.Any<IndividualEnrollment>(), Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow<IndividualEnrollment>(
                cancellationTokenSource,
                new ProvisioningServiceClientException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.CreateIndividualTpmEnrollment(EndorsementKey, RegistrationId, DeviceId, tags: null, desiredProperties: null, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<ProvisioningServiceClientException>();
    }

    [Fact]
    public async Task DeleteIndividualEnrollment_EnrollmentExists_DeletesItAndReturnsTrue()
    {
        // Act
        var actual = await sut.DeleteIndividualEnrollment(RegistrationId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeTrue();

        await client
            .Received(1)
            .DeleteIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteIndividualEnrollment_EnrollmentIsAlreadyGone_ReturnsFalse()
    {
        // Arrange
        client
            .DeleteIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .ThrowsAsync(CreateDpsError(HttpStatusCode.NotFound));

        // Act
        var actual = await sut.DeleteIndividualEnrollment(RegistrationId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteIndividualEnrollment_DpsRejectsTheKey_ThrowsTheDpsError()
    {
        // Arrange
        client
            .DeleteIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .ThrowsAsync(CreateDpsError(HttpStatusCode.Unauthorized));

        // Act
        var act = () => sut.DeleteIndividualEnrollment(RegistrationId, TestContext.Current.CancellationToken);

        // Assert
        (await act.Should().ThrowAsync<ProvisioningServiceClientHttpException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteIndividualEnrollment_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheDpsError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The DPS SDK reports the caller's cancellation as a ProvisioningServiceClientException.
        client
            .DeleteIndividualEnrollment(RegistrationId, Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow(
                cancellationTokenSource,
                new ProvisioningServiceClientException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.DeleteIndividualEnrollment(RegistrationId, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<ProvisioningServiceClientException>();
    }

    private static IndividualEnrollment CreateEnrollment()
        => new(RegistrationId, new TpmAttestation(EndorsementKey));

    /// <summary>
    /// Creates the exception the DPS SDK throws for an HTTP error. The SDK sets its status code in an internal
    /// constructor only, so it is set through reflection here.
    /// </summary>
    private static ProvisioningServiceClientHttpException CreateDpsError(
        HttpStatusCode statusCode)
    {
        var exception = new ProvisioningServiceClientHttpException($"DPS answered {statusCode}");
        typeof(ProvisioningServiceClientHttpException)
            .GetProperty(nameof(ProvisioningServiceClientHttpException.StatusCode))!
            .SetValue(exception, statusCode);

        return exception;
    }
}