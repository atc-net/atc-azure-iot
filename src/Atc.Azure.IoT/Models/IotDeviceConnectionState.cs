namespace Atc.Azure.IoT.Models;

/// <summary>
/// Specifies the different connection states of an iot device.
/// </summary>
public enum IotDeviceConnectionState
{
    /// <summary>
    /// Represents a device in the Disconnected state.
    /// </summary>
    Disconnected = 0,

    /// <summary>
    /// Represents a device in the Connected state.
    /// </summary>
    Connected = 1,
}