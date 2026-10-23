namespace Atc.Azure.IoTEdge.DeviceEmulator.Tests.Provisioning;

public sealed class IoTEdgeDeviceProvisionerTests
{
    private const string IotHubHostName = "myhub.azure-devices.net";
    private const string DeviceId = "mydevice";
    private const string ModuleId = "mymodule";
    private const string EdgeHubModuleId = "$edgeHub";

    private readonly IIoTHubService iotHubService = Substitute.For<IIoTHubService>();
    private readonly IoTEdgeDeviceProvisioner provisioner;

    public IoTEdgeDeviceProvisionerTests()
    {
        provisioner = new IoTEdgeDeviceProvisioner(iotHubService, IotHubHostName);

        iotHubService
            .GetDevice(DeviceId, Arg.Any<CancellationToken>())
            .Returns(new Device(DeviceId) { Capabilities = new DeviceCapabilities { IotEdge = true } });

        iotHubService
            .UpdateModule(Arg.Any<Module>(), Arg.Any<CancellationToken>())
            .Returns(x => (true, x.Arg<Module>()));
    }

    [Fact]
    public async Task Provision_EdgeHubWithoutCredentials_AssignsSymmetricKeys()
    {
        // IoT Hub creates $edgeHub with authentication type 'none'.
        iotHubService
            .GetModulesOnIotEdgeDevice(DeviceId, Arg.Any<CancellationToken>())
            .Returns([CreateModule(ModuleId, "modulekey"), new Module(DeviceId, EdgeHubModuleId)]);

        var identity = await provisioner.Provision(DeviceId, [ModuleId], deploymentManifest: null, CancellationToken.None);

        await iotHubService
            .Received(1)
            .UpdateModule(
                Arg.Is<Module>(x =>
                    x.Id == EdgeHubModuleId &&
                    x.Authentication.Type == AuthenticationType.Sas &&
                    !string.IsNullOrEmpty(x.Authentication.SymmetricKey.PrimaryKey) &&
                    !string.IsNullOrEmpty(x.Authentication.SymmetricKey.SecondaryKey)),
                Arg.Any<CancellationToken>());

        identity.GetEdgeHubConnectionString().Should().Contain($"ModuleId={EdgeHubModuleId};SharedAccessKey=");
        identity.GetModuleConnectionString(ModuleId, gatewayHostName: null).Should().EndWith("SharedAccessKey=modulekey");
    }

    [Fact]
    public async Task Provision_ModulesWithKeys_DoesNotUpdateModules()
    {
        iotHubService
            .GetModulesOnIotEdgeDevice(DeviceId, Arg.Any<CancellationToken>())
            .Returns([CreateModule(ModuleId, "modulekey"), CreateModule(EdgeHubModuleId, "edgehubkey")]);

        var identity = await provisioner.Provision(DeviceId, [ModuleId], deploymentManifest: null, CancellationToken.None);

        await iotHubService
            .DidNotReceive()
            .UpdateModule(Arg.Any<Module>(), Arg.Any<CancellationToken>());

        identity.GetEdgeHubConnectionString().Should().EndWith("SharedAccessKey=edgehubkey");
    }

    [Fact]
    public Task Provision_AssigningKeysFails_Throws()
    {
        iotHubService
            .GetModulesOnIotEdgeDevice(DeviceId, Arg.Any<CancellationToken>())
            .Returns([CreateModule(ModuleId, "modulekey"), new Module(DeviceId, EdgeHubModuleId)]);

        iotHubService
            .UpdateModule(Arg.Any<Module>(), Arg.Any<CancellationToken>())
            .Returns((false, (Module?)null));

        var act = () => provisioner.Provision(DeviceId, [ModuleId], deploymentManifest: null, CancellationToken.None);

        return act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*'$edgeHub'*");
    }

    private static Module CreateModule(
        string moduleId,
        string primaryKey)
        => new(DeviceId, moduleId)
        {
            Authentication = new AuthenticationMechanism
            {
                Type = AuthenticationType.Sas,
                SymmetricKey = new SymmetricKey
                {
                    PrimaryKey = primaryKey,
                    SecondaryKey = primaryKey,
                },
            },
        };
}