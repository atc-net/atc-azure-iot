namespace Atc.Azure.IoT.Options;

public sealed class DeviceProvisioningServiceOptions
{
    // TODO: Fill out
    public string ConnectionString { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(ConnectionString)}: {ConnectionString}";
}