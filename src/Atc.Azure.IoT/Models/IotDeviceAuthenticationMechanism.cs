namespace Atc.Azure.IoT.Models;

public class IotDeviceAuthenticationMechanism
{
    public IotDeviceAuthenticationType AuthenticationType { get; set; }

    public SymmetricKey? SymmetricKey { get; set; }

    public X509Thumbprint? X509Thumbprint { get; set; }
}