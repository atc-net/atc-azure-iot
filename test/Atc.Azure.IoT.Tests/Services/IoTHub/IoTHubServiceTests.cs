// The Atc.Azure.IoT.Models namespace also has a Module type, so this namespace is imported here only.
using Microsoft.Azure.Devices;

namespace Atc.Azure.IoT.Tests.Services.IoTHub;

public sealed class IoTHubServiceTests : IDisposable
{
    private const string IotHubHostName = "test-hub.azure-devices.net";
    private const string DeviceId = "device-1";
    private const string ModuleId = "module-1";

    private readonly RegistryManager registryManager = Substitute.For<RegistryManager>();
    private readonly IIoTHubModuleService iotHubModuleService = Substitute.For<IIoTHubModuleService>();
    private readonly IoTHubService sut;

    public IoTHubServiceTests()
        => sut = new IoTHubService(iotHubModuleService, registryManager, IotHubHostName);

    public void Dispose()
    {
        sut.Dispose();
        registryManager.Dispose();
    }

    [Fact]
    public async Task GetDeviceRegistryStatistics_ReturnsTheStatisticsFromIoTHub()
    {
        // Arrange
        var statistics = new RegistryStatistics();
        registryManager
            .GetRegistryStatisticsAsync(Arg.Any<CancellationToken>())
            .Returns(statistics);

        // Act
        var actual = await sut.GetDeviceRegistryStatistics(TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(statistics);
    }

    [Fact]
    public Task GetDeviceRegistryStatistics_IoTHubRejectsTheKey_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .GetRegistryStatisticsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new UnauthorizedException("The SAS token has an invalid signature"));

        // Act
        var act = () => sut.GetDeviceRegistryStatistics(TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateDevice_CreatesTheDeviceWithTheRequestedIoTEdgeCapability(bool edgeEnabled)
    {
        // Arrange
        var created = new Device(DeviceId);
        registryManager
            .AddDeviceAsync(Arg.Any<Device>(), Arg.Any<CancellationToken>())
            .Returns(created);

        // Act
        var actual = await sut.CreateDevice(DeviceId, edgeEnabled, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(created);

        await registryManager
            .Received(1)
            .AddDeviceAsync(
                Arg.Is<Device>(d => d.Id == DeviceId && d.Capabilities.IotEdge == edgeEnabled),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public Task CreateDevice_DeviceAlreadyExists_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .AddDeviceAsync(Arg.Any<Device>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceAlreadyExistsException(DeviceId));

        // Act
        var act = () => sut.CreateDevice(DeviceId, edgeEnabled: true, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<DeviceAlreadyExistsException>();
    }

    [Fact]
    public async Task GetDevice_DeviceExists_ReturnsTheDevice()
    {
        // Arrange
        var device = new Device(DeviceId);
        registryManager
            .GetDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns(device);

        // Act
        var actual = await sut.GetDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(device);
    }

    [Fact]
    public async Task GetDevice_DeviceDoesNotExist_ReturnsNull()
    {
        // Arrange
        registryManager
            .GetDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        // Act
        var actual = await sut.GetDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public Task GetDevice_IoTHubRejectsTheKey_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .GetDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new UnauthorizedException("The SAS token has an invalid signature"));

        // Act
        var act = () => sut.GetDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task GetDevice_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheIoTHubError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The IoT Hub SDK reports the caller's cancellation as an IotHubException.
        registryManager
            .GetDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow<Device?>(
                cancellationTokenSource,
                new IotHubException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.GetDevice(DeviceId, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<IotHubException>();
    }

    [Fact]
    public async Task GetDeviceConnectionString_DeviceExists_ReturnsConnectionStringWithThePrimaryKey()
    {
        // Arrange
        var primaryKey = Convert.ToBase64String(new byte[32]);
        registryManager
            .GetDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns(CreateDeviceWithPrimaryKey(primaryKey));

        // Act
        var actual = await sut.GetDeviceConnectionString(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().Be($"HostName={IotHubHostName};DeviceId={DeviceId};SharedAccessKey={primaryKey}");
    }

    [Fact]
    public async Task GetDeviceConnectionString_DeviceDoesNotExist_ReturnsNull()
    {
        // Arrange
        registryManager
            .GetDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        // Act
        var actual = await sut.GetDeviceConnectionString(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeNull();
    }

    [Theory]
    [InlineData(true, "SELECT * FROM devices WHERE capabilities.iotEdge = true")]
    [InlineData(false, "SELECT * FROM devices")]
    public async Task GetDeviceTwins_QueriesOnlyIoTEdgeDevicesWhenAsked(
        bool onlyIncludeEdgeDevices,
        string expectedQuery)
    {
        // Arrange
        var query = CreateQuery([]);
        registryManager
            .CreateQuery(Arg.Any<string>(), Arg.Any<int?>())
            .Returns(query);

        // Act
        await sut.GetDeviceTwins(onlyIncludeEdgeDevices, TestContext.Current.CancellationToken);

        // Assert
        registryManager
            .Received(1)
            .CreateQuery(expectedQuery, Arg.Any<int?>());
    }

    [Fact]
    public async Task GetDeviceTwins_ResultHasSeveralPages_ReturnsTheTwinsOfAllPages()
    {
        // Arrange
        var query = CreateQuery(
        [
            [new Twin("device-1"), new Twin("device-2")],
            [new Twin("device-3")],
        ]);

        registryManager
            .CreateQuery(Arg.Any<string>(), Arg.Any<int?>())
            .Returns(query);

        // Act
        var actual = await sut.GetDeviceTwins(onlyIncludeEdgeDevices: true, TestContext.Current.CancellationToken);

        // Assert
        actual
            .Select(x => x.DeviceId)
            .Should()
            .Equal("device-1", "device-2", "device-3");
    }

    [Fact]
    public async Task GetDeviceTwins_NoDevices_ReturnsAnEmptyList()
    {
        // Arrange
        var query = CreateQuery([]);
        registryManager
            .CreateQuery(Arg.Any<string>(), Arg.Any<int?>())
            .Returns(query);

        // Act
        var actual = await sut.GetDeviceTwins(onlyIncludeEdgeDevices: true, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public Task GetDeviceTwins_IoTHubFailsOnTheSecondPage_ThrowsInsteadOfReturningThePartialList()
    {
        // Arrange
        var query = Substitute.For<IQuery>();
        query.HasMoreResults.Returns(true);
        query
            .GetNextAsTwinAsync()
            .Returns(
                _ => Task.FromResult<IEnumerable<Twin>>([new Twin("device-1")]),
                _ => throw new ThrottlingException("Too many requests"));

        registryManager
            .CreateQuery(Arg.Any<string>(), Arg.Any<int?>())
            .Returns(query);

        // Act
        var act = () => sut.GetDeviceTwins(onlyIncludeEdgeDevices: true, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<ThrottlingException>();
    }

    [Fact]
    public async Task GetDeviceTwins_CancelledBetweenPages_StopsAndThrowsOperationCanceledException()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var query = Substitute.For<IQuery>();
        query.HasMoreResults.Returns(true);
        query
            .GetNextAsTwinAsync()
            .Returns(async _ =>
            {
                await cancellationTokenSource.CancelAsync();
                return (IEnumerable<Twin>)[new Twin("device-1")];
            });

        registryManager
            .CreateQuery(Arg.Any<string>(), Arg.Any<int?>())
            .Returns(query);

        // Act
        var act = () => sut.GetDeviceTwins(onlyIncludeEdgeDevices: true, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        await query
            .Received(1)
            .GetNextAsTwinAsync();
    }

    [Fact]
    public async Task GetDeviceTwin_DeviceExists_ReturnsTheTwin()
    {
        // Arrange
        var twin = new Twin(DeviceId);
        registryManager
            .GetTwinAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns(twin);

        // Act
        var actual = await sut.GetDeviceTwin(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(twin);
    }

    [Fact]
    public async Task GetDeviceTwin_DeviceDoesNotExist_ReturnsNull()
    {
        // Arrange
        registryManager
            .GetTwinAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns((Twin?)null);

        // Act
        var actual = await sut.GetDeviceTwin(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task UpdateDeviceTwin_SendsTheTwinWithItsETagAndReturnsTheUpdatedTwin()
    {
        // Arrange
        var twin = new Twin(DeviceId) { ETag = "etag-1" };
        var updated = new Twin(DeviceId);
        registryManager
            .UpdateTwinAsync(DeviceId, twin, "etag-1", Arg.Any<CancellationToken>())
            .Returns(updated);

        // Act
        var actual = await sut.UpdateDeviceTwin(DeviceId, twin, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(updated);
    }

    [Fact]
    public Task UpdateDeviceTwin_TwinWasChangedSinceItWasRead_ThrowsTheIoTHubError()
    {
        // Arrange
        var twin = new Twin(DeviceId) { ETag = "etag-1" };
        registryManager
            .UpdateTwinAsync(DeviceId, twin, "etag-1", Arg.Any<CancellationToken>())
            .ThrowsAsync(new PreconditionFailedException("The ETag doesn't match"));

        // Act
        var act = () => sut.UpdateDeviceTwin(DeviceId, twin, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<PreconditionFailedException>();
    }

    [Fact]
    public async Task GetModulesOnIotEdgeDevice_DeviceHasModules_ReturnsTheModuleIdentities()
    {
        // Arrange
        var modules = new[]
        {
            new Microsoft.Azure.Devices.Module(DeviceId, "$edgeAgent"),
            new Microsoft.Azure.Devices.Module(DeviceId, ModuleId),
        };

        registryManager
            .GetModulesOnDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns(modules);

        // Act
        var actual = await sut.GetModulesOnIotEdgeDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().Equal(modules);
    }

    [Fact]
    public async Task GetModulesOnIotEdgeDevice_DeviceHasNoModules_ReturnsAnEmptyList()
    {
        // Arrange
        registryManager
            .GetModulesOnDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var actual = await sut.GetModulesOnIotEdgeDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public Task GetModulesOnIotEdgeDevice_DeviceDoesNotExist_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .GetModulesOnDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceNotFoundException(DeviceId));

        // Act
        var act = () => sut.GetModulesOnIotEdgeDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<DeviceNotFoundException>();
    }

    [Fact]
    public async Task UpdateDesiredProperties_SendsOnlyTheDesiredPropertiesAsAPatchForAnyTwinVersion()
    {
        // Arrange
        var desired = new TwinCollection("""{ "interval": 5, "removed": null }""");
        var updated = new Twin(DeviceId);
        Twin? sentPatch = null;
        registryManager
            .UpdateTwinAsync(DeviceId, ModuleId, Arg.Do<Twin>(x => sentPatch = x), "*", Arg.Any<CancellationToken>())
            .Returns(updated);

        // Act
        var actual = await sut.UpdateDesiredProperties(DeviceId, ModuleId, desired, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(updated);
        sentPatch.Should().NotBeNull();
        sentPatch!.Properties.Desired.Should().BeSameAs(desired);
        sentPatch.Properties.Reported.Count.Should().Be(0);
        sentPatch.Tags.Count.Should().Be(0);
    }

    [Fact]
    public Task UpdateDesiredProperties_ModuleDoesNotExist_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .UpdateTwinAsync(DeviceId, ModuleId, Arg.Any<Twin>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ModuleNotFoundException(DeviceId, ModuleId));

        // Act
        var act = () => sut.UpdateDesiredProperties(DeviceId, ModuleId, new TwinCollection(), TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<ModuleNotFoundException>();
    }

    [Fact]
    public async Task GetModuleTwin_ModuleExists_ReturnsTheTwin()
    {
        // Arrange
        var twin = new Twin(DeviceId);
        registryManager
            .GetTwinAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .Returns(twin);

        // Act
        var actual = await sut.GetModuleTwin(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(twin);
    }

    [Fact]
    public async Task GetModuleTwin_ModuleDoesNotExist_ReturnsNull()
    {
        // Arrange
        registryManager
            .GetTwinAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .Returns((Twin?)null);

        // Act
        var actual = await sut.GetModuleTwin(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public Task GetModuleTwin_IoTHubIsThrottling_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .GetTwinAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new ThrottlingException("Too many requests"));

        // Act
        var act = () => sut.GetModuleTwin(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<ThrottlingException>();
    }

    [Fact]
    public async Task AddModuleToDevice_ReturnsTheCreatedModuleIdentity()
    {
        // Arrange
        var created = new Microsoft.Azure.Devices.Module(DeviceId, ModuleId);
        registryManager
            .AddModuleAsync(Arg.Any<Microsoft.Azure.Devices.Module>(), Arg.Any<CancellationToken>())
            .Returns(created);

        // Act
        var actual = await sut.AddModuleToDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(created);

        await registryManager
            .Received(1)
            .AddModuleAsync(
                Arg.Is<Microsoft.Azure.Devices.Module>(m => m.DeviceId == DeviceId && m.Id == ModuleId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public Task AddModuleToDevice_ModuleAlreadyExists_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .AddModuleAsync(Arg.Any<Microsoft.Azure.Devices.Module>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ModuleAlreadyExistsException(DeviceId, ModuleId));

        // Act
        var act = () => sut.AddModuleToDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<ModuleAlreadyExistsException>();
    }

    [Fact]
    public async Task UpdateModule_ReturnsTheUpdatedModuleIdentity()
    {
        // Arrange
        var module = new Microsoft.Azure.Devices.Module(DeviceId, ModuleId);
        var updated = new Microsoft.Azure.Devices.Module(DeviceId, ModuleId);
        registryManager
            .UpdateModuleAsync(module, Arg.Any<CancellationToken>())
            .Returns(updated);

        // Act
        var actual = await sut.UpdateModule(module, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(updated);
    }

    [Fact]
    public async Task RemoveModuleFromDevice_ModuleExists_RemovesItAndReturnsTrue()
    {
        // Act
        var actual = await sut.RemoveModuleFromDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeTrue();

        await registryManager
            .Received(1)
            .RemoveModuleAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveModuleFromDevice_ModuleIsAlreadyGone_ReturnsFalse()
    {
        // Arrange
        // IoT Hub reports a module that doesn't exist as DeviceNotFoundException.
        registryManager
            .RemoveModuleAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceNotFoundException(DeviceId));

        // Act
        var actual = await sut.RemoveModuleFromDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveModuleFromDevice_ModuleNotFoundError_ReturnsFalse()
    {
        // Arrange
        registryManager
            .RemoveModuleAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new ModuleNotFoundException(DeviceId, ModuleId));

        // Act
        var actual = await sut.RemoveModuleFromDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public Task RemoveModuleFromDevice_IoTHubRejectsTheKey_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .RemoveModuleAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new UnauthorizedException("The SAS token has an invalid signature"));

        // Act
        var act = () => sut.RemoveModuleFromDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RemoveModuleFromDevice_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheIoTHubError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The IoT Hub SDK reports the caller's cancellation as an IotHubException.
        registryManager
            .RemoveModuleAsync(DeviceId, ModuleId, Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow(
                cancellationTokenSource,
                new IotHubException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.RemoveModuleFromDevice(DeviceId, ModuleId, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<IotHubException>();
    }

    [Fact]
    public async Task RestartModuleOnDevice_EdgeAgentRestartsTheModule_ReturnsSucceededWithAMessage()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(200, "null"));

        // Act
        var (succeeded, statusCode, jsonPayload) = await sut.RestartModuleOnDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        succeeded.Should().BeTrue();
        statusCode.Should().Be(200);
        jsonPayload.Should().Contain($"Successfully restarted module '{ModuleId}' on device '{DeviceId}'");

        await iotHubModuleService
            .Received(1)
            .CallMethod(
                DeviceId,
                "$edgeAgent",
                Arg.Is<MethodParameterModel>(p => p.Name == "RestartModule" && p.JsonPayload.Contains($"\"id\":\"{ModuleId}\"")),
                Arg.Any<IoTHubRequestOptions?>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RestartModuleOnDevice_EdgeAgentAnswersWithAnError_ReturnsNotSucceededWithEdgeAgentsAnswer()
    {
        // Arrange
        const string edgeAgentAnswer = """{ "message": "Module not found" }""";
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(404, edgeAgentAnswer));

        // Act
        var (succeeded, statusCode, jsonPayload) = await sut.RestartModuleOnDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        succeeded.Should().BeFalse();
        statusCode.Should().Be(404);
        jsonPayload.Should().Be(edgeAgentAnswer);
    }

    [Fact]
    public Task RestartModuleOnDevice_DeviceIsOffline_ThrowsTheIoTHubError()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceNotFoundException(ErrorCode.DeviceNotOnline, "The device is offline"));

        // Act
        var act = () => sut.RestartModuleOnDevice(DeviceId, ModuleId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<DeviceNotFoundException>();
    }

    [Fact]
    public async Task UploadModuleLogs_SendsTheRequestToEdgeAgentWithLowercaseEnumValues()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(200, """{ "status": "running", "message": "", "correlationId": "abc" }"""));

        var request = new UploadModuleLogsRequest(
            new Uri("https://storage.example/logs"),
            [new LogRequestItem(".*")],
            LogContentEncoding.Gzip,
            LogContentType.Json);

        // Act
        await sut.UploadModuleLogs(DeviceId, request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await iotHubModuleService
            .Received(1)
            .CallMethod(
                DeviceId,
                "$edgeAgent",
                Arg.Is<MethodParameterModel>(p =>
                    p.Name == "UploadModuleLogs" &&
                    p.JsonPayload.Contains("\"encoding\":\"gzip\"") &&
                    p.JsonPayload.Contains("\"contentType\":\"json\"")),
                Arg.Any<IoTHubRequestOptions?>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UploadModuleLogs_EdgeAgentStartsTheUpload_ReturnsItsStatusAndCorrelationId()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(200, """{ "status": "running", "message": "", "correlationId": "abc" }"""));

        // Act
        var actual = await sut.UploadModuleLogs(DeviceId, CreateUploadModuleLogsRequest(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.StatusCode.Should().Be(200);
        actual.Payload.Status.Should().Be(BackgroundTaskRunStatus.Running);
        actual.Payload.CorrelationId.Should().Be("abc");
    }

    [Fact]
    public async Task UploadModuleLogs_DeviceIsOffline_ReturnsStatus404WithTheIoTHubErrorInsteadOfThrowing()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceNotFoundException(ErrorCode.DeviceNotOnline, "The device is offline"));

        // Act
        var actual = await sut.UploadModuleLogs(DeviceId, CreateUploadModuleLogsRequest(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.StatusCode.Should().Be(404);
        actual.Payload.Status.Should().Be(BackgroundTaskRunStatus.Unknown);
        actual.Payload.Message.Should().Contain("The device is offline");
        actual.Payload.ErrorCode.Should().Be(nameof(ErrorCode.DeviceNotOnline));
    }

    [Fact]
    public Task UploadModuleLogs_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfReturningAResponse()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var act = () => sut.UploadModuleLogs(DeviceId, CreateUploadModuleLogsRequest(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task UploadSupportBundle_SendsTheRequestToEdgeAgent()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(200, """{ "status": "running", "message": "", "correlationId": "abc" }"""));

        // Act
        await sut.UploadSupportBundle(
            DeviceId,
            new UploadSupportBundleRequest(new Uri("https://storage.example/logs"), EdgeRuntimeOnly: true),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await iotHubModuleService
            .Received(1)
            .CallMethod(
                DeviceId,
                "$edgeAgent",
                Arg.Is<MethodParameterModel>(p => p.Name == "UploadSupportBundle" && p.JsonPayload.Contains("\"edgeRuntimeOnly\":true")),
                Arg.Any<IoTHubRequestOptions?>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTaskStatus_EdgeAgentReportsTheTaskAsCompleted_ReturnsCompleted()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(200, """{ "status": "completed", "message": "", "correlationId": "abc" }"""));

        // Act
        var actual = await sut.GetTaskStatus(DeviceId, new GetTaskStatusRequest("abc"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.StatusCode.Should().Be(200);
        actual.Payload.Status.Should().Be(BackgroundTaskRunStatus.Completed);

        await iotHubModuleService
            .Received(1)
            .CallMethod(
                DeviceId,
                "$edgeAgent",
                Arg.Is<MethodParameterModel>(p => p.Name == "GetTaskStatus" && p.JsonPayload.Contains("\"correlationId\":\"abc\"")),
                Arg.Any<IoTHubRequestOptions?>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTaskStatus_EdgeAgentAnswersWithoutPayload_ReturnsUnknownStatus()
    {
        // Arrange
        iotHubModuleService
            .CallMethod(DeviceId, "$edgeAgent", Arg.Any<MethodParameterModel>(), Arg.Any<IoTHubRequestOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new MethodResultModel(500, string.Empty));

        // Act
        var actual = await sut.GetTaskStatus(DeviceId, new GetTaskStatusRequest("abc"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.StatusCode.Should().Be(500);
        actual.Payload.Status.Should().Be(BackgroundTaskRunStatus.Unknown);
    }

    [Fact]
    public async Task ApplyConfigurationContentOnDevice_AppliesTheContentToTheDevice()
    {
        // Arrange
        var content = new ConfigurationContent();

        // Act
        await sut.ApplyConfigurationContentOnDevice(DeviceId, content, TestContext.Current.CancellationToken);

        // Assert
        await registryManager
            .Received(1)
            .ApplyConfigurationContentOnDeviceAsync(DeviceId, content, Arg.Any<CancellationToken>());
    }

    [Fact]
    public Task ApplyConfigurationContentOnDevice_IoTHubRejectsTheContent_ThrowsTheIoTHubError()
    {
        // Arrange
        var content = new ConfigurationContent();
        registryManager
            .ApplyConfigurationContentOnDeviceAsync(DeviceId, content, Arg.Any<CancellationToken>())
            .ThrowsAsync(new IotHubException("The deployment manifest is invalid"));

        // Act
        var act = () => sut.ApplyConfigurationContentOnDevice(DeviceId, content, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<IotHubException>();
    }

    [Fact]
    public async Task AddNewModules_AddsAModuleIdentityForEachModuleExceptTheSystemModules()
    {
        // Arrange
        var content = CreateConfigurationContent("$edgeAgent", "$edgeHub", "module-a", "module-b");

        // Act
        await sut.AddNewModules(DeviceId, content, TestContext.Current.CancellationToken);

        // Assert
        await registryManager
            .Received(2)
            .AddModuleAsync(Arg.Any<Microsoft.Azure.Devices.Module>(), Arg.Any<CancellationToken>());

        await registryManager
            .Received(1)
            .AddModuleAsync(Arg.Is<Microsoft.Azure.Devices.Module>(m => m.DeviceId == DeviceId && m.Id == "module-a"), Arg.Any<CancellationToken>());

        await registryManager
            .Received(1)
            .AddModuleAsync(Arg.Is<Microsoft.Azure.Devices.Module>(m => m.DeviceId == DeviceId && m.Id == "module-b"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public Task AddNewModules_ModuleAlreadyExists_ThrowsTheIoTHubError()
    {
        // Arrange
        var content = CreateConfigurationContent("module-a");
        registryManager
            .AddModuleAsync(Arg.Any<Microsoft.Azure.Devices.Module>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ModuleAlreadyExistsException(DeviceId, "module-a"));

        // Act
        var act = () => sut.AddNewModules(DeviceId, content, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<ModuleAlreadyExistsException>();
    }

    [Fact]
    public async Task DeleteDevice_DeviceExists_DeletesItAndReturnsTrue()
    {
        // Act
        var actual = await sut.DeleteDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeTrue();

        await registryManager
            .Received(1)
            .RemoveDeviceAsync(DeviceId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteDevice_DeviceIsAlreadyGone_ReturnsFalse()
    {
        // Arrange
        registryManager
            .RemoveDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceNotFoundException(DeviceId));

        // Act
        var actual = await sut.DeleteDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public Task DeleteDevice_IoTHubRejectsTheKey_ThrowsTheIoTHubError()
    {
        // Arrange
        registryManager
            .RemoveDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new UnauthorizedException("The SAS token has an invalid signature"));

        // Act
        var act = () => sut.DeleteDevice(DeviceId, TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task DeleteDevice_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheIoTHubError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The IoT Hub SDK reports the caller's cancellation as an IotHubException.
        registryManager
            .RemoveDeviceAsync(DeviceId, Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow(
                cancellationTokenSource,
                new IotHubException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.DeleteDevice(DeviceId, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<IotHubException>();
    }

    private static Device CreateDeviceWithPrimaryKey(string primaryKey)
        => new(DeviceId)
        {
            Authentication = new AuthenticationMechanism
            {
                SymmetricKey = new SymmetricKey
                {
                    PrimaryKey = primaryKey,
                    SecondaryKey = primaryKey,
                },
            },
        };

    private static IQuery CreateQuery(
        IReadOnlyList<Twin[]> pages)
    {
        var query = Substitute.For<IQuery>();
        var hasMoreResults = Enumerable
            .Repeat(true, pages.Count)
            .Append(false)
            .ToArray();

        query.HasMoreResults.Returns(hasMoreResults[0], hasMoreResults[1..]);

        if (pages.Count > 0)
        {
            var results = pages
                .Select(x => Task.FromResult<IEnumerable<Twin>>(x))
                .ToArray();

            query
                .GetNextAsTwinAsync()
                .Returns(results[0], results[1..]);
        }

        return query;
    }

    private static UploadModuleLogsRequest CreateUploadModuleLogsRequest()
        => new(
            new Uri("https://storage.example/logs"),
            [new LogRequestItem(".*")]);

    private static ConfigurationContent CreateConfigurationContent(
        params string[] moduleIds)
        => new()
        {
            ModulesContent = moduleIds.ToDictionary(
                x => x,
                _ => (IDictionary<string, object>)new Dictionary<string, object>(StringComparer.Ordinal),
                StringComparer.Ordinal),
        };
}