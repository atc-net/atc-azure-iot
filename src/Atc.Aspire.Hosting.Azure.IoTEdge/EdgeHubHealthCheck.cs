namespace Atc.Aspire.Hosting.Azure.IoTEdge;

/// <summary>
/// Reports edgeHub as healthy once its MQTT head completes a TLS handshake.
/// A plain TCP connect is not enough, because the container runtime accepts connections on published ports
/// before edgeHub itself listens.
/// </summary>
internal sealed class EdgeHubHealthCheck : IHealthCheck
{
    private readonly int port;

    public EdgeHubHealthCheck(int port)
    {
        this.port = port;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("localhost", port, cancellationToken);

            await using var sslStream = new SslStream(tcpClient.GetStream(), leaveInnerStreamOpen: false);
            await sslStream.AuthenticateAsClientAsync(
                new SslClientAuthenticationOptions
                {
                    TargetHost = "localhost",

                    // Only liveness is checked here; modules validate the certificate chain themselves.
                    RemoteCertificateValidationCallback = (_, _, _, _) => true,
                },
                cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex) when (ex is SocketException or IOException or System.Security.Authentication.AuthenticationException)
        {
            return HealthCheckResult.Unhealthy($"edgeHub is not accepting TLS connections on port {port} yet.", ex);
        }
    }
}