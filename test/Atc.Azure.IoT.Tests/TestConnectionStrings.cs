namespace Atc.Azure.IoT.Tests;

/// <summary>
/// Connection strings in the right format for tests. The key is built at runtime from zero bytes, so no key-like
/// literal is committed.
/// </summary>
internal static class TestConnectionStrings
{
    public static readonly string Key = Convert.ToBase64String(new byte[32]);

    public static readonly string IotHub = $"HostName=test-hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey={Key}";

    public static readonly string DeviceProvisioningService = $"HostName=test-dps.azure-devices-provisioning.net;SharedAccessKeyName=provisioningserviceowner;SharedAccessKey={Key}";
}