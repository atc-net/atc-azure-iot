namespace Atc.Azure.IoT.Models;

public class IoTDeviceCapabilities
{
    [JsonPropertyName("iotEdge")]
    public bool IotEdge { get; set; }
}