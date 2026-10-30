namespace Atc.Azure.IoTEdge.Tests.TestMocks;

/// <summary>
/// <see cref="MockTwinProperties"/> is static, so every test that changes it restores it.
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
    public async Task GetTwinAsync_ReturnsTheMockTwinProperties()
    {
        // Arrange
        MockTwinProperties.Desired = new TwinCollection("""{ "interval": 5 }""");
        MockTwinProperties.Reported = new TwinCollection("""{ "status": "ok" }""");

        // Act
        var actual = await sut.GetTwinAsync(TestContext.Current.CancellationToken);

        // Assert
        ((int)actual.Properties.Desired["interval"]).Should().Be(5);
        ((string)actual.Properties.Reported["status"]).Should().Be("ok");
    }

    [Fact]
    public async Task UpdateReportedPropertiesAsync_StoresThemSoTheTwinReturnsThem()
    {
        // Arrange
        var reported = new TwinCollection("""{ "status": "running" }""");

        // Act
        await sut.UpdateReportedPropertiesAsync(reported, TestContext.Current.CancellationToken);

        // Assert
        var twin = await sut.GetTwinAsync(TestContext.Current.CancellationToken);
        ((string)twin.Properties.Reported["status"]).Should().Be("running");
    }

    [Fact]
    public async Task UpdateReportedPropertiesAsync_DesiredPropertyCallbackSet_CallsItWithTheDesiredProperties()
    {
        // Arrange
        MockTwinProperties.Desired = new TwinCollection("""{ "interval": 5 }""");
        TwinCollection? received = null;

        await sut.SetDesiredPropertyUpdateCallbackAsync(
            (desired, _) =>
            {
                received = desired;
                return Task.CompletedTask;
            },
            userContext: null!,
            TestContext.Current.CancellationToken);

        // Act
        await sut.UpdateReportedPropertiesAsync(new TwinCollection(), TestContext.Current.CancellationToken);

        // Assert
        received.Should().BeSameAs(MockTwinProperties.Desired);
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
    public Task OpenSendAndClose_CompleteWithoutAConnection()
    {
        // Act
        var act = async () =>
        {
            await sut.OpenAsync(TestContext.Current.CancellationToken);
            await sut.SendEventAsync("output1", new Message(Encoding.UTF8.GetBytes("{}")), TestContext.Current.CancellationToken);
            await sut.SendEventBatchAsync([new Message(), new Message()], TestContext.Current.CancellationToken);
            await sut.CloseAsync(TestContext.Current.CancellationToken);
        };

        // Assert
        return act.Should().NotThrowAsync();
    }
}