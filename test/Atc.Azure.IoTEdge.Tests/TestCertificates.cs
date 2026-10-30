namespace Atc.Azure.IoTEdge.Tests;

/// <summary>
/// Creates a CA and a server certificate signed by it, like an IoT Edge device's edge CA and edgeHub's certificate.
/// </summary>
internal sealed class TestCertificates : IDisposable
{
    public TestCertificates(string caName = "Test Edge CA")
    {
        var now = DateTimeOffset.UtcNow;

        using var caKey = RSA.Create(2048);
        var caRequest = new CertificateRequest($"CN={caName}", caKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        caRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(certificateAuthority: true, hasPathLengthConstraint: false, pathLengthConstraint: 0, critical: true));
        caRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, critical: true));
        Ca = caRequest.CreateSelfSigned(now.AddDays(-1), now.AddDays(30));

        using var serverKey = RSA.Create(2048);
        var serverRequest = new CertificateRequest("CN=edgehub", serverKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        serverRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(certificateAuthority: false, hasPathLengthConstraint: false, pathLengthConstraint: 0, critical: false));
        Server = serverRequest.Create(Ca, now.AddDays(-1), now.AddDays(10), [1, 2, 3, 4]);
    }

    public X509Certificate2 Ca { get; }

    public X509Certificate2 Server { get; }

    public void Dispose()
    {
        Server.Dispose();
        Ca.Dispose();
    }
}