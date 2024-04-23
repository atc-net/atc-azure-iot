namespace Atc.Azure.IoT.Options;

public sealed class DeviceProvisioningServiceOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string IdScope { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(ConnectionString)}: {ConnectionString}, {nameof(IdScope)}: {IdScope}";
}