namespace Atc.Azure.IoT.Models;

public class IotDevice
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("etag")]
    public string Etag { get; set; } = string.Empty;

    [JsonPropertyName("deviceEtag")]
    public string DeviceEtag { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public IotDeviceStatus Status { get; set; }

    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; } = string.Empty;

    [JsonPropertyName("statusUpdateTime")]
    public DateTimeOffset? StatusUpdateTime { get; set; }

    [JsonPropertyName("connectionState")]
    public IotDeviceConnectionState ConnectionState { get; set; }

    [JsonPropertyName("lastActivityTime")]
    public DateTimeOffset? LastActivityTime { get; set; }

    [JsonPropertyName("authenticationType")]
    public IotDeviceAuthenticationType AuthenticationType { get; set; }

    [JsonPropertyName("capabilities")]
    public IoTDeviceCapabilities Capabilities { get; set; } = new();
}