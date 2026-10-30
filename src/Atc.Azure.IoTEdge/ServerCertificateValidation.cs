namespace Atc.Azure.IoTEdge;

/// <summary>
/// How a module checks the TLS certificate of the server it connects to: edgeHub, or IoT Hub directly.
/// </summary>
/// <remarks>
/// The values start at 1, so a value that was never set (<c>default</c>) is rejected instead of meaning
/// <see cref="AcceptAny"/>.
/// </remarks>
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "OK - a value that was never set must be rejected, not mean one of the choices.")]
public enum ServerCertificateValidation
{
    /// <summary>
    /// Accept any certificate, without checking it. The connection is still encrypted, but the module can't tell the
    /// real server from anything in between.
    /// </summary>
    AcceptAny = 1,

    /// <summary>
    /// Accept any certificate, but log a warning when the normal validation (<see cref="Validate"/>) would have refused
    /// it, and why. Use it to try out validation in production without risk.
    /// </summary>
    Audit = 2,

    /// <summary>
    /// The device SDK's normal validation: the certificate must be for the host name the module connects to, and be
    /// signed by a trusted root (for edgeHub, the IoT Edge device's edge CA). Revocation isn't checked, unless
    /// <c>CertificateRevocationCheck</c> is turned on in the transport settings.
    /// </summary>
    Validate = 3,
}