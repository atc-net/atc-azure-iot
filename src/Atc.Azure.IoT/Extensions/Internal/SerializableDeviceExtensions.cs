namespace Atc.Azure.IoT.Extensions.Internal;

internal static class SerializableDeviceExtensions
{
    public static IotDevice ToIotDevice(
        this SerializableDevice serializableDevice)
        => new()
        {
            DeviceId = serializableDevice.DeviceId,
            Etag = serializableDevice.DeviceEtag,
            Status = serializableDevice.Status,
            StatusReason = serializableDevice.StatusReason,
            StatusUpdateTime = serializableDevice.StatusUpdateTime,
            ConnectionState = serializableDevice.ConnectionState,
            LastActivityTime = serializableDevice.LastActivityTime,
            AuthenticationType = serializableDevice.AuthenticationType,
            IotEdge = serializableDevice.Capabilities?.IotEdge ?? false,
        };
}