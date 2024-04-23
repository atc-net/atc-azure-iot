namespace Atc.Azure.IoT.Options;

public sealed class IotHubOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(ConnectionString)}: {ConnectionString}";
}