namespace Atc.Azure.IoTEdge.DeviceEmulator.Certificates;

/// <summary>
/// The certificate files needed to run edgeHub outside the IoT Edge runtime.
/// </summary>
/// <param name="DirectoryPath">The directory holding the files.</param>
/// <param name="DeviceCaCertificateFile">The root CA (PEM) that modules trust.</param>
/// <param name="ChainCertificateFile">The CA chain (PEM) edgeHub presents together with its server certificate.</param>
/// <param name="ServerCertificateFile">The edgeHub server certificate including its private key (PFX without password).</param>
public sealed record EdgeHubCertificateFiles(
    string DirectoryPath,
    string DeviceCaCertificateFile,
    string ChainCertificateFile,
    string ServerCertificateFile)
{
    public const string DeviceCaCertificateFileName = "edge-device-ca.cert.pem";
    public const string ChainCertificateFileName = "edge-chain-ca.cert.pem";
    public const string ServerCertificateFileName = "edge-hub-server.cert.pfx";

    public static EdgeHubCertificateFiles ForDirectory(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);

        return new EdgeHubCertificateFiles(
            directoryPath,
            Path.Combine(directoryPath, DeviceCaCertificateFileName),
            Path.Combine(directoryPath, ChainCertificateFileName),
            Path.Combine(directoryPath, ServerCertificateFileName));
    }
}