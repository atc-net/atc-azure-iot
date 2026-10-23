namespace Atc.Azure.IoT.Options;

/// <summary>
/// Options for connecting to an Azure IoT Hub Device Provisioning Service (DPS) with a service connection string.
/// </summary>
public sealed class DeviceProvisioningServiceOptions
{
    /// <summary>
    /// The default configuration section name, e.g. <c>DeviceProvisioningService:ConnectionString</c>
    /// or the environment variable <c>DeviceProvisioningService__ConnectionString</c>.
    /// </summary>
    public const string SectionName = "DeviceProvisioningService";

    /// <summary>
    /// Gets or sets the DPS service connection string, e.g. <c>HostName=&lt;dps&gt;.azure-devices-provisioning.net;SharedAccessKeyName=&lt;policy&gt;;SharedAccessKey=&lt;key&gt;</c>.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID scope of the DPS instance, which devices use to register.
    /// </summary>
    public string IdScope { get; set; } = string.Empty;

    /// <inheritdoc />
    /// <remarks>The connection string is redacted, since it holds the shared access key.</remarks>
    public override string ToString()
        => $"{nameof(ConnectionString)}: {ConnectionStringRedactor.Redact(ConnectionString)}, {nameof(IdScope)}: {IdScope}";
}