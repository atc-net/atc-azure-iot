namespace Atc.Azure.IoTEdge.DeviceEmulator.Factories;

public static class TransportSettingsFactory
{
    public static AmqpTransportSettings BuildEmulatorTransportSettings()
        => new(Microsoft.Azure.Devices.Client.TransportType.Amqp_Tcp_Only)
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
}