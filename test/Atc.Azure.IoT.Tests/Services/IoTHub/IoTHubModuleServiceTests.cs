// The Atc.Azure.IoT.Models namespace also has a Module type, so this namespace is imported here only.
using Microsoft.Azure.Devices;

namespace Atc.Azure.IoT.Tests.Services.IoTHub;

public sealed class IoTHubModuleServiceTests : IDisposable
{
    private const string DeviceId = "device-1";
    private const string ModuleId = "module-1";

    private static readonly IoTHubRequestOptions NoDelayRetries = new()
    {
        MaxRetryAttempts = 2,
        Delay = TimeSpan.Zero,
    };

    private readonly ServiceClient serviceClient = Substitute.For<ServiceClient>();
    private readonly IoTHubModuleService sut;

    public IoTHubModuleServiceTests()
        => sut = new IoTHubModuleService(serviceClient);

    public void Dispose()
    {
        sut.Dispose();
        serviceClient.Dispose();
    }

    [Fact]
    public async Task CallMethod_ModuleAnswers_ReturnsItsStatusAndPayload()
    {
        // Arrange
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>())
            .Returns(CreateResult(200, """{"temperature":21}"""));

        // Act
        var actual = await sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("GetTemperature", "{}"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.Status.Should().Be(200);
        actual.JsonPayload.Should().Be("""{"temperature":21}""");
    }

    [Fact]
    public async Task CallMethod_ModuleAnswersWithAnError_ReturnsTheAnswerInsteadOfThrowing()
    {
        // Arrange
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>())
            .Returns(CreateResult(501, "null"));

        // Act
        var actual = await sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("Unknown", "{}"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.Status.Should().Be(501);
    }

    [Fact]
    public async Task CallMethod_SendsTheMethodNameAndPayloadToTheModule()
    {
        // Arrange
        CloudToDeviceMethod? sentMethod = null;
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Do<CloudToDeviceMethod>(x => sentMethod = x), Arg.Any<CancellationToken>())
            .Returns(CreateResult(200, "null"));

        // Act
        await sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("SetInterval", """{"seconds":5}"""), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        sentMethod.Should().NotBeNull();
        sentMethod!.MethodName.Should().Be("SetInterval");
        sentMethod.GetPayloadAsJson().Should().Be("""{"seconds":5}""");
    }

    [Fact]
    public async Task CallMethod_TimeoutInRequestOptions_IsHowLongTheModuleHasToAnswer()
    {
        // Arrange
        CloudToDeviceMethod? sentMethod = null;
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Do<CloudToDeviceMethod>(x => sentMethod = x), Arg.Any<CancellationToken>())
            .Returns(CreateResult(200, "null"));

        var requestOptions = new IoTHubRequestOptions { Timeout = TimeSpan.FromSeconds(10) };

        // Act
        await sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("Ping", "{}"), requestOptions, TestContext.Current.CancellationToken);

        // Assert
        sentMethod.Should().NotBeNull();
        sentMethod!.ResponseTimeout.Should().Be(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task CallMethod_IoTHubFailsOnceWithATransientError_RetriesAndReturnsTheAnswer()
    {
        // Arrange
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>())
            .Returns(
                _ => throw new IotHubException("IoT Hub is busy", isTransient: true),
                _ => Task.FromResult(CreateResult(200, "null")));

        // Act
        var actual = await sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("Ping", "{}"), NoDelayRetries, TestContext.Current.CancellationToken);

        // Assert
        actual.Status.Should().Be(200);

        await serviceClient
            .Received(2)
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMethod_TransientErrorRemainsAfterTheRetries_ThrowsIt()
    {
        // Arrange
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IotHubException("IoT Hub is busy", isTransient: true));

        // Act
        var act = () => sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("Ping", "{}"), NoDelayRetries, TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<IotHubException>();

        // The first call and the 2 retries.
        await serviceClient
            .Received(3)
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMethod_DeviceIsOffline_ThrowsWithoutRetrying()
    {
        // Arrange
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new DeviceNotFoundException(ErrorCode.DeviceNotOnline, "The device is offline"));

        // Act
        var act = () => sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("Ping", "{}"), NoDelayRetries, TestContext.Current.CancellationToken);

        // Assert
        (await act.Should().ThrowAsync<DeviceNotFoundException>())
            .Which.Code.Should().Be(ErrorCode.DeviceNotOnline);

        await serviceClient
            .Received(1)
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMethod_CallerCancels_ThrowsOperationCanceledExceptionInsteadOfTheIoTHubError()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // The IoT Hub SDK reports the caller's cancellation as an IotHubException.
        serviceClient
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>())
            .Returns(_ => SdkCancellation.CancelThenThrow<CloudToDeviceMethodResult>(
                cancellationTokenSource,
                new IotHubException("A task was canceled.", new TaskCanceledException())));

        // Act
        var act = () => sut.CallMethod(DeviceId, ModuleId, new MethodParameterModel("Ping", "{}"), NoDelayRetries, cancellationTokenSource.Token);

        // Assert
        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.InnerException.Should().BeOfType<IotHubException>();

        await serviceClient
            .Received(1)
            .InvokeDeviceMethodAsync(DeviceId, ModuleId, Arg.Any<CloudToDeviceMethod>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public Task CallMethod_NoModuleId_ThrowsArgumentException()
    {
        // Act
        var act = () => sut.CallMethod(DeviceId, string.Empty, new MethodParameterModel("Ping", "{}"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    /// <summary>
    /// Creates a method result as the SDK reads it from IoT Hub; its payload can only be set through JSON.
    /// </summary>
    private static CloudToDeviceMethodResult CreateResult(
        int status,
        string jsonPayload)
        => Newtonsoft.Json.JsonConvert.DeserializeObject<CloudToDeviceMethodResult>(
            $$"""{"status":{{status}},"payload":{{jsonPayload}}}""")!;
}