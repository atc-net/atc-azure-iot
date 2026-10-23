namespace Atc.Azure.IoT.Options;

/// <summary>
/// Options for connecting to an Azure IoT Hub with a service connection string.
/// </summary>
public sealed class IotHubOptions
{
    /// <summary>
    /// The default configuration section name, e.g. <c>IotHub:ConnectionString</c> or the environment variable <c>IotHub__ConnectionString</c>.
    /// </summary>
    public const string SectionName = "IotHub";

    /// <summary>
    /// Gets or sets the IoT Hub service connection string, e.g. <c>HostName=&lt;hub&gt;.azure-devices.net;SharedAccessKeyName=&lt;policy&gt;;SharedAccessKey=&lt;key&gt;</c>.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <inheritdoc />
    /// <remarks>The connection string is redacted, since it holds the shared access key.</remarks>
    public override string ToString()
        => $"{nameof(ConnectionString)}: {ConnectionStringRedactor.Redact(ConnectionString)}";
}