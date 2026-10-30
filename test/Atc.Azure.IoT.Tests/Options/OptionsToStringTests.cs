namespace Atc.Azure.IoT.Tests.Options;

public sealed class OptionsToStringTests
{
    private const string ConnectionString = "HostName=myhub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=c2VjcmV0";

    [Fact]
    public void IotHubOptions_ToString_RedactsSharedAccessKey()
        => new IotHubOptions { ConnectionString = ConnectionString }
            .ToString()
            .Should().Be("ConnectionString: HostName=myhub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=***");

    [Fact]
    public void DeviceProvisioningServiceOptions_ToString_RedactsSharedAccessKey()
        => new DeviceProvisioningServiceOptions { ConnectionString = ConnectionString, IdScope = "0ne00000000" }
            .ToString()
            .Should().Be("ConnectionString: HostName=myhub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=***, IdScope: 0ne00000000");

    [Theory]
    [InlineData("")]
    [InlineData("not a connection string")]
    public void IotHubOptions_ToString_ConnectionStringWithoutSecrets_ShowsItUnchanged(string connectionString)
        => new IotHubOptions { ConnectionString = connectionString }
            .ToString()
            .Should().Be($"ConnectionString: {connectionString}");
}