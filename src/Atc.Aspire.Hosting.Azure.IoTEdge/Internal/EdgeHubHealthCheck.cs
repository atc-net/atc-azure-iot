namespace Atc.Aspire.Hosting.Azure.IoTEdge.Internal;

/// <summary>
/// Reports edgeHub as healthy once its MQTT head completes a TLS handshake.
/// A plain TCP connect is not enough, because the container runtime accepts connections on published ports
/// before edgeHub itself listens.
/// </summary>
/// <param name="mqttPort">The host port edgeHub's MQTT head is published on.</param>
internal sealed class EdgeHubHealthCheck(int mqttPort) : IHealthCheck
{
    /// <summary>
    /// Connects to the edgeHub MQTT port on the host and completes a TLS handshake.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A plain TCP connect would report edgeHub as healthy too early: the container runtime accepts connections on a
    /// published port as soon as the container starts, before edgeHub listens inside it. Modules started at that moment
    /// would fail their first connection.
    /// </para>
    /// <para>
    /// A TLS handshake only completes when edgeHub itself answers. The server certificate is accepted without validation,
    /// because the check is only about liveness; modules validate the certificate chain themselves.
    /// </para>
    /// <para>
    /// The check runs in the AppHost process, so it connects to <c>localhost</c> on the port published on the host.
    /// </para>
    /// </remarks>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Healthy when the TLS handshake completes; otherwise unhealthy.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("localhost", mqttPort, cancellationToken);

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
            return HealthCheckResult.Unhealthy($"edgeHub is not accepting TLS connections on MQTT port {mqttPort} yet.", ex);
        }
    }
}