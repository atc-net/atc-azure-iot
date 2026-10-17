namespace Atc.Azure.IoTEdge.DeviceEmulator.Tests.Provisioning;

public sealed class IoTEdgeDeviceIdentityTests
{
    private readonly IoTEdgeDeviceIdentity identity = new(
        "myhub.azure-devices.net",
        "mydevice",
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["$edgeHub"] = "edgehubkey",
            ["mymodule"] = "modulekey",
        });

    [Fact]
    public void GetEdgeHubConnectionString_ReturnsEdgeHubModuleConnectionString()
        => identity.GetEdgeHubConnectionString()
            .Should().Be("HostName=myhub.azure-devices.net;DeviceId=mydevice;ModuleId=$edgeHub;SharedAccessKey=edgehubkey");

    [Fact]
    public void GetModuleConnectionString_WithoutGateway_ConnectsDirectlyToIoTHub()
        => identity.GetModuleConnectionString("mymodule", gatewayHostName: null)
            .Should().Be("HostName=myhub.azure-devices.net;DeviceId=mydevice;ModuleId=mymodule;SharedAccessKey=modulekey");

    [Fact]
    public void GetModuleConnectionString_WithGateway_IncludesGatewayHostName()
        => identity.GetModuleConnectionString("mymodule", "edgehub")
            .Should().Be("HostName=myhub.azure-devices.net;GatewayHostName=edgehub;DeviceId=mydevice;ModuleId=mymodule;SharedAccessKey=modulekey");

    [Fact]
    public void GetModuleConnectionString_UnknownModule_Throws()
    {
        // Act
        var act = () => identity.GetModuleConnectionString("unknown", gatewayHostName: null);

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }
}