namespace Atc.Azure.IoT.Models;

public class IotDevice
{
    public string DeviceId { get; set; } = string.Empty;

    public string Etag { get; set; } = string.Empty;

    public IotDeviceStatus Status { get; set; }

    public string StatusReason { get; set; } = string.Empty;

    public DateTimeOffset? StatusUpdateTime { get; set; }

    public IotDeviceConnectionState ConnectionState { get; set; }

    public DateTimeOffset? LastActivityTime { get; set; }

    public IotDeviceAuthenticationMechanism AuthenticationMechanism { get; set; } = new();

    public bool IotEdge { get; set; }

    public string RawJson { get; set; } = string.Empty;
}