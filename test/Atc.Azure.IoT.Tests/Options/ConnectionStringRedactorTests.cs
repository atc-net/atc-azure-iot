namespace Atc.Azure.IoT.Tests.Options;

public sealed class ConnectionStringRedactorTests
{
    [Fact]
    public void Redact_SharedAccessKey_IsReplacedByStars()
        => ConnectionStringRedactor.Redact($"HostName=hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey={TestConnectionStrings.Key}")
            .Should().Be("HostName=hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=***");

    [Fact]
    public void Redact_SharedAccessSignature_IsReplacedByStars()
        => ConnectionStringRedactor.Redact("HostName=hub.azure-devices.net;SharedAccessSignature=SharedAccessSignature sr=hub&sig=abc&se=1")
            .Should().Be("HostName=hub.azure-devices.net;SharedAccessSignature=***");

    [Fact]
    public void Redact_KeyNameInOtherCase_IsStillReplaced()
        => ConnectionStringRedactor.Redact($"HostName=hub.azure-devices.net;sharedaccesskey={TestConnectionStrings.Key}")
            .Should().Be("HostName=hub.azure-devices.net;sharedaccesskey=***");

    [Fact]
    public void Redact_KeyEndingWithEqualsSigns_IsReplacedCompletely()
        => ConnectionStringRedactor.Redact("HostName=hub.azure-devices.net;SharedAccessKey=abc==")
            .Should().Be("HostName=hub.azure-devices.net;SharedAccessKey=***");

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("HostName=hub.azure-devices.net;DeviceId=device-1", "HostName=hub.azure-devices.net;DeviceId=device-1")]
    public void Redact_NoSecrets_KeepsTheText(
        string? connectionString,
        string expected)
        => ConnectionStringRedactor.Redact(connectionString).Should().Be(expected);
}