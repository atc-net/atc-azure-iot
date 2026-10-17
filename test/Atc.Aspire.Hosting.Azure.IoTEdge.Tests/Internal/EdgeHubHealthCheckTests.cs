namespace Atc.Aspire.Hosting.Azure.IoTEdge.Tests.Internal;

public sealed class EdgeHubHealthCheckTests : IDisposable
{
    private readonly DirectoryInfo certificateDirectory = Directory.CreateTempSubdirectory("atc-iotedge-healthcheck-tests-");

    public void Dispose()
        => certificateDirectory.Delete(recursive: true);

    [Fact]
    public async Task CheckHealthAsync_NothingListening_ReturnsUnhealthy()
    {
        // Arrange
        var healthCheck = new EdgeHubHealthCheck(GetFreePort());

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task CheckHealthAsync_ServerAcceptsTcpWithoutTls_ReturnsUnhealthy()
    {
        // Arrange
        // Like a published container port before edgeHub listens: the connection is accepted, then closed.
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var server = AcceptAndCloseAsync(listener, TestContext.Current.CancellationToken);
        var healthCheck = new EdgeHubHealthCheck(((IPEndPoint)listener.LocalEndpoint).Port);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);
        await server;

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task CheckHealthAsync_ServerCompletesTlsHandshake_ReturnsHealthy()
    {
        // Arrange
        var files = EdgeHubCertificateGenerator.GenerateCertificates(certificateDirectory.FullName, ["localhost"]);
        using var serverCertificate = X509CertificateLoader.LoadPkcs12FromFile(files.ServerCertificateFile, password: null);

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var server = AcceptTlsAsync(listener, serverCertificate, TestContext.Current.CancellationToken);
        var healthCheck = new EdgeHubHealthCheck(((IPEndPoint)listener.LocalEndpoint).Port);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);
        await server;

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    private static async Task AcceptAndCloseAsync(
        TcpListener listener,
        CancellationToken cancellationToken)
    {
        using var client = await listener.AcceptTcpClientAsync(cancellationToken);
    }

    private static async Task AcceptTlsAsync(
        TcpListener listener,
        X509Certificate2 certificate,
        CancellationToken cancellationToken)
    {
        using var client = await listener.AcceptTcpClientAsync(cancellationToken);
        await using var sslStream = new SslStream(client.GetStream());
        await sslStream.AuthenticateAsServerAsync(
            new SslServerAuthenticationOptions { ServerCertificate = certificate },
            cancellationToken);
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}