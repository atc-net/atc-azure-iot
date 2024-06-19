namespace Atc.Azure.IoT.Extensions;

public static class DeviceExtensions
{
    public static IotDevice ToIotDevice(
        this Device device)
    {
        var iotDevice = new IotDevice
        {
            DeviceId = device.Id,
            Etag = device.ETag,
            StatusReason = device.StatusReason,
            StatusUpdateTime = new DateTimeOffset(device.StatusUpdatedTime),
            LastActivityTime = new DateTimeOffset(device.LastActivityTime),
        };

        if (Enum<IotDeviceConnectionState>.TryParse(device.ConnectionState.ToString(), out var connectionState))
        {
            iotDevice.ConnectionState = connectionState;
        }

        if (Enum<IotDeviceStatus>.TryParse(device.Status.ToString(), out var status))
        {
            iotDevice.Status = status;
        }

        if (device.Authentication is not null &&
            Enum<IotDeviceAuthenticationType>.TryParse(device.Authentication.Type.ToString(), out var authenticationType))
        {
            iotDevice.AuthenticationType = authenticationType;
        }

        if (device.Capabilities is not null)
        {
            iotDevice.IotEdge = device.Capabilities.IotEdge;
        }

        return iotDevice;
    }
}