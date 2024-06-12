namespace Atc.Azure.IoT.Models;

/// <summary>
/// Specifies the different states of an iot device.
/// </summary>
public enum IotDeviceStatus
{
    /// <summary>
    /// Indicates that a Device is enabled
    /// </summary>
    [EnumMember(Value = "enabled")]
    Enabled = 0,

    /// <summary>
    /// Indicates that a Device is disabled
    /// </summary>
    [EnumMember(Value = "disabled")]
    Disabled,
}