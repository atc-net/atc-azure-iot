namespace Atc.Azure.IoTEdge.Tests.TestMocks;

/// <summary>
/// <see cref="MockTwinProperties"/> is static, so the tests that change it restore it.
/// </summary>
public sealed class MockModuleClientWrapperTests : IDisposable
{
    private readonly TwinCollection originalDesired = MockTwinProperties.Desired;
    private readonly TwinCollection originalReported = MockTwinProperties.Reported;
    private readonly MockModuleClientWrapper sut = new(NullLogger<MockModuleClientWrapper>.Instance);

    public void Dispose()
    {
        MockTwinProperties.Desired = originalDesired;
        MockTwinProperties.Reported = originalReported;
        sut.Dispose();
    }

    [Fact]
    public async Task GetTwinAsync_NewMock_StartsWithTheMockTwinProperties()
    {
        // Arrange
        MockTwinProperties.Desired = new TwinCollection("""{ "interval": 5 }""");
        MockTwinProperties.Reported = new TwinCollection("""{ "status": "ok" }""");
        using var mock = CreateMock();

        // Act
        var actual = await mock.GetTwinAsync(TestContext.Current.CancellationToken);

        // Assert
        ((int)actual.Properties.Desired["interval"]).Should().Be(5);
        ((string)actual.Properties.Reported["status"]).Should().Be("ok");
    }

    [Fact]
    public async Task GetTwinAsync_MockTwinPropertiesChangedAfterTheMockWasCreated_KeepsItsOwnTwin()
    {
        // Arrange
        MockTwinProperties.Desired = new TwinCollection("""{ "interval": 5 }""");
        using var mock = CreateMock();
        MockTwinProperties.Desired = new TwinCollection("""{ "interval": 60 }""");

        // Act
        var actual = await mock.GetTwinAsync(TestContext.Current.CancellationToken);

        // Assert
        ((int)actual.Properties.Desired["interval"]).Should().Be(5);
    }

    [Fact]
    public async Task UpdateReportedPropertiesAsync_MergesThePatchIntoTheReportedProperties()
    {
        // Arrange
        await sut.UpdateReportedPropertiesAsync(new TwinCollection("""{ "status": "starting", "version": "1.0" }"""), TestContext.Current.CancellationToken);

        // Act
        await sut.UpdateReportedPropertiesAsync(new TwinCollection("""{ "status": "running" }"""), TestContext.Current.CancellationToken);

        // Assert
        var twin = await sut.GetTwinAsync(TestContext.Current.CancellationToken);
        ((string)twin.Properties.Reported["status"]).Should().Be("running");
        ((string)twin.Properties.Reported["version"]).Should().Be("1.0");
    }

    [Fact]
    public async Task UpdateReportedPropertiesAsync_PropertySetToNull_RemovesIt()
    {
        // Arrange
        await sut.UpdateReportedPropertiesAsync(new TwinCollection("""{ "status": "running", "error": "timeout" }"""), TestContext.Current.CancellationToken);

        // Act
        await sut.UpdateReportedPropertiesAsync(new TwinCollection("""{ "error": null }"""), TestContext.Current.CancellationToken);

        // Assert
        var twin = await sut.GetTwinAsync(TestContext.Current.CancellationToken);
        twin.Properties.Reported.Contains("error").Should().BeFalse();
        twin.Properties.Reported.Contains("status").Should().BeTrue();
    }

    [Fact]
    public async Task UpdateReportedPropertiesAsync_DoesNotCallTheDesiredPropertyCallback()
    {
        // Arrange
        var calls = 0;
        await sut.SetDesiredPropertyUpdateCallbackAsync(
            (_, _) =>
            {
                calls++;
                return Task.CompletedTask;
            },
            userContext: null!,
            TestContext.Current.CancellationToken);

        // Act
        await sut.UpdateReportedPropertiesAsync(new TwinCollection("""{ "status": "running" }"""), TestContext.Current.CancellationToken);

        // Assert
        calls.Should().Be(0);
    }

    [Fact]
    public async Task UpdateReportedPropertiesAsync_DoesNotChangeOtherMocks()
    {
        // Arrange
        using var otherMock = CreateMock();

        // Act
        await sut.UpdateReportedPropertiesAsync(new TwinCollection("""{ "status": "running" }"""), TestContext.Current.CancellationToken);

        // Assert
        var otherTwin = await otherMock.GetTwinAsync(TestContext.Current.CancellationToken);
        otherTwin.Properties.Reported.Contains("status").Should().BeFalse();
    }

    [Fact]
    public async Task SimulateDesiredPropertiesUpdateAsync_MergesThePatchAndPassesItToTheCallback()
    {
        // Arrange
        MockTwinProperties.Desired = new TwinCollection("""{ "interval": 5, "unit": "seconds" }""");
        using var mock = CreateMock();
        TwinCollection? received = null;

        await mock.SetDesiredPropertyUpdateCallbackAsync(
            (patch, _) =>
            {
                received = patch;
                return Task.CompletedTask;
            },
            userContext: null!,
            TestContext.Current.CancellationToken);

        var desiredPatch = new TwinCollection("""{ "interval": 10 }""");

        // Act
        await mock.SimulateDesiredPropertiesUpdateAsync(desiredPatch);

        // Assert
        received.Should().BeSameAs(desiredPatch);

        var twin = await mock.GetTwinAsync(TestContext.Current.CancellationToken);
        ((int)twin.Properties.Desired["interval"]).Should().Be(10);
        ((string)twin.Properties.Desired["unit"]).Should().Be("seconds");
    }

    [Fact]
    public async Task SendEventAsync_ToAnOutput_IsInTheSentMessagesOfThatOutput()
    {
        // Arrange
        using var message = new Message(Encoding.UTF8.GetBytes("""{ "temperature": 21 }"""));

        // Act
        await sut.SendEventAsync("telemetry", message, TestContext.Current.CancellationToken);

        // Assert
        sut.GetSentMessages("telemetry").Should().Equal(message);
        sut.GetSentMessages().Should().BeEmpty();
    }

    [Fact]
    public async Task SendEventAsync_WithoutAnOutput_GoesToTheDefaultOutput()
    {
        // Arrange
        using var message = new Message();

        // Act
        await sut.SendEventAsync(message, TestContext.Current.CancellationToken);

        // Assert
        sut.GetSentMessages(MockModuleClientWrapper.DefaultOutputName).Should().Equal(message);
    }

    [Fact]
    public async Task SendEventBatchAsync_KeepsTheOrderOfTheMessages()
    {
        // Arrange
        using var first = new Message();
        using var second = new Message();

        // Act
        await sut.SendEventBatchAsync("telemetry", [first, second], TestContext.Current.CancellationToken);

        // Assert
        sut.GetSentMessages("telemetry").Should().Equal(first, second);
    }

    [Fact]
    public async Task SimulateMethodCallAsync_HandlerForTheMethod_GetsTheRequestAndItsUserContext()
    {
        // Arrange
        var userContext = new object();
        MethodRequest? receivedRequest = null;
        object? receivedContext = null;

        await sut.SetMethodHandlerAsync(
            "SetInterval",
            (request, context) =>
            {
                receivedRequest = request;
                receivedContext = context;
                return Task.FromResult(new MethodResponse(200));
            },
            userContext,
            TestContext.Current.CancellationToken);

        // Act
        var actual = await sut.SimulateMethodCallAsync("SetInterval", """{"seconds":5}""");

        // Assert
        actual.Status.Should().Be(200);
        receivedRequest!.DataAsJson.Should().Be("""{"seconds":5}""");
        receivedContext.Should().BeSameAs(userContext);
    }

    [Fact]
    public async Task SimulateMethodCallAsync_OnlyADefaultHandler_UsesIt()
    {
        // Arrange
        await sut.SetMethodDefaultHandlerAsync(
            (request, _) => Task.FromResult(new MethodResponse(request.Name == "Ping" ? 200 : 404)),
            userContext: null!,
            TestContext.Current.CancellationToken);

        // Act
        var actual = await sut.SimulateMethodCallAsync("Ping");

        // Assert
        actual.Status.Should().Be(200);
    }

    [Fact]
    public async Task SimulateMethodCallAsync_NoHandler_ReturnsStatus501AsIoTHubDoes()
    {
        // Act
        var actual = await sut.SimulateMethodCallAsync("Unknown");

        // Assert
        actual.Status.Should().Be(501);
    }

    [Fact]
    public async Task SimulateInputMessageAsync_HandlerForTheInput_GetsTheMessage()
    {
        // Arrange
        using var message = new Message();
        Message? received = null;

        await sut.SetInputMessageHandlerAsync(
            "input1",
            (m, _) =>
            {
                received = m;
                return Task.FromResult(MessageResponse.Completed);
            },
            userContext: null!,
            TestContext.Current.CancellationToken);

        // Act
        var actual = await sut.SimulateInputMessageAsync("input1", message);

        // Assert
        actual.Should().Be(MessageResponse.Completed);
        received.Should().BeSameAs(message);
    }

    [Fact]
    public async Task SimulateInputMessageAsync_OnlyADefaultHandler_UsesIt()
    {
        // Arrange
        using var message = new Message();
        await sut.SetMessageHandlerAsync(
            (_, _) => Task.FromResult(MessageResponse.Abandoned),
            userContext: null!,
            TestContext.Current.CancellationToken);

        // Act
        var actual = await sut.SimulateInputMessageAsync("input1", message);

        // Assert
        actual.Should().Be(MessageResponse.Abandoned);
    }

    [Fact]
    public async Task SimulateInputMessageAsync_NoHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        using var message = new Message();

        // Act
        var act = () => sut.SimulateInputMessageAsync("input1", message);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("The module has no message handler for input input1");
    }

    [Fact]
    public void SimulateConnectionStatusChange_CallsTheHandlerWithTheStatusAndReason()
    {
        // Arrange
        (ConnectionStatus Status, ConnectionStatusChangeReason Reason)? received = null;
        sut.SetConnectionStatusChangesHandler((status, reason) => received = (status, reason));

        // Act
        sut.SimulateConnectionStatusChange(ConnectionStatus.Disconnected_Retrying, ConnectionStatusChangeReason.Communication_Error);

        // Assert
        received.Should().Be((ConnectionStatus.Disconnected_Retrying, ConnectionStatusChangeReason.Communication_Error));
    }

    [Fact]
    public void SetRetryPolicy_IsAvailableAsRetryPolicy()
    {
        // Arrange
        var retryPolicy = new NoRetry();

        // Act
        sut.SetRetryPolicy(retryPolicy);

        // Assert
        sut.RetryPolicy.Should().BeSameAs(retryPolicy);
    }

    [Fact]
    public async Task InvokeMethodAsync_SimulatedResponseSet_ReturnsIt()
    {
        // Arrange
        var response = new MethodResponse(200);
        sut.SetMethodResponse("Ping", response);

        // Act
        var actual = await sut.InvokeMethodAsync("device-1", "module-1", new MethodRequest("Ping"), TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeSameAs(response);
    }

    [Fact]
    public Task InvokeMethodAsync_NoSimulatedResponse_ThrowsInvalidOperationException()
    {
        // Act
        var act = () => sut.InvokeMethodAsync("device-1", "module-1", new MethodRequest("Ping"), TestContext.Current.CancellationToken);

        // Assert
        return act.Should().ThrowAsync<InvalidOperationException>().WithMessage("No simulated response for method Ping");
    }

    [Fact]
    public Task OpenAndClose_CompleteWithoutAConnection()
    {
        // Act
        var act = async () =>
        {
            await sut.OpenAsync(TestContext.Current.CancellationToken);
            await sut.CloseAsync(TestContext.Current.CancellationToken);
        };

        // Assert
        return act.Should().NotThrowAsync();
    }

    private static MockModuleClientWrapper CreateMock()
        => new(NullLogger<MockModuleClientWrapper>.Instance);
}