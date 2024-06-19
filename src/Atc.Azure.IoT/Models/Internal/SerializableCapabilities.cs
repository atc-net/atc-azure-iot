namespace Atc.Azure.IoT.Models.Internal;

public class SerializableCapabilities
{
    [JsonPropertyName("iotEdge")]
    public bool IotEdge { get; set; }
}