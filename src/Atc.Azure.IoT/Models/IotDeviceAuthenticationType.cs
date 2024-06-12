namespace Atc.Azure.IoT.Models;

/// <summary>
/// Iot Device's authentication mechanism
/// </summary>
public enum IotDeviceAuthenticationType
{
    /// <summary>
    /// Shared Access Key
    /// </summary>
    [EnumMember(Value = "sas")]
    Sas = 0,

    /// <summary>
    /// Self-signed certificate
    /// </summary>
    [EnumMember(Value = "selfSigned")]
    SelfSigned = 1,

    /// <summary>
    /// Certificate Authority
    /// </summary>
    [EnumMember(Value = "certificateAuthority")]
    CertificateAuthority = 2,

    /// <summary>
    /// No Authentication Token at this scope
    /// </summary>
    [EnumMember(Value = "none")]
    None = 3,
}