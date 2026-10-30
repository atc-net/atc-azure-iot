namespace Atc.Azure.IoTEdge.Factories;

/// <summary>
/// The certificate check behind <see cref="ServerCertificateValidation.Audit"/>: it accepts every certificate, and logs
/// what the normal validation would have refused.
/// </summary>
internal static partial class ServerCertificateAudit
{
    /// <summary>
    /// The environment variable IoT Edge (and the Aspire integration) sets to the edge CA certificate file.
    /// </summary>
    public const string EdgeCaCertificateFileVariableName = "EdgeModuleCACertificateFile";

    /// <summary>
    /// Creates the callback for the transport settings.
    /// </summary>
    /// <param name="logger">The logger for the warnings.</param>
    /// <param name="edgeCaCertificate">The edge CA the module trusts, if any.</param>
    /// <returns>A callback that always accepts the certificate.</returns>
    public static RemoteCertificateValidationCallback CreateCallback(
        ILogger logger,
        X509Certificate2? edgeCaCertificate)
        => (_, certificate, chain, sslPolicyErrors) =>
        {
            var problem = GetValidationProblem(certificate, chain, sslPolicyErrors, edgeCaCertificate);
            if (problem is not null)
            {
                LogServerCertificateValidationWouldFail(logger, certificate?.Subject ?? "(no certificate)", problem);
            }

            return true;
        };

    /// <summary>
    /// Gets what the normal validation would refuse, or <see langword="null"/> if it would accept the certificate.
    /// </summary>
    /// <remarks>
    /// On Linux, the device SDK installs the edge CA in the certificate store, so the platform's result is enough. On
    /// Windows it doesn't, and the SDK checks the chain against the edge CA itself; this method does the same, so a
    /// certificate signed by the edge CA isn't reported.
    /// </remarks>
    /// <param name="certificate">The server certificate.</param>
    /// <param name="chain">The chain the platform built for it.</param>
    /// <param name="sslPolicyErrors">The platform's validation result.</param>
    /// <param name="edgeCaCertificate">The edge CA the module trusts, if any.</param>
    /// <returns>The problem, for example <c>RemoteCertificateNameMismatch</c> or <c>UntrustedRoot</c>, or <see langword="null"/>.</returns>
    public static string? GetValidationProblem(
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors,
        X509Certificate2? edgeCaCertificate)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            return null;
        }

        if (certificate is null)
        {
            return nameof(SslPolicyErrors.RemoteCertificateNotAvailable);
        }

        var otherErrors = sslPolicyErrors & ~SslPolicyErrors.RemoteCertificateChainErrors;
        if (otherErrors != SslPolicyErrors.None)
        {
            return otherErrors.ToString();
        }

        if (edgeCaCertificate is not null &&
            IsSignedBy(certificate, edgeCaCertificate))
        {
            return null;
        }

        var chainStatuses = chain?.ChainStatus
            .Select(x => x.Status.ToString())
            .Distinct(StringComparer.Ordinal)
            .ToList() ?? [];

        return chainStatuses.Count > 0
            ? string.Join(", ", chainStatuses)
            : nameof(SslPolicyErrors.RemoteCertificateChainErrors);
    }

    /// <summary>
    /// Loads the edge CA certificate from the file in <see cref="EdgeCaCertificateFileVariableName"/>.
    /// </summary>
    /// <param name="filePath">The certificate file (PEM), or <see langword="null"/> when the variable isn't set.</param>
    /// <param name="logger">The logger for a file that can't be read.</param>
    /// <returns>The edge CA certificate, or <see langword="null"/> when there is none or it can't be read.</returns>
    public static X509Certificate2? LoadEdgeCaCertificate(
        string? filePath,
        ILogger logger)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        try
        {
            // The file only holds the certificate (no private key), as PEM.
            return X509Certificate2.CreateFromPem(File.ReadAllText(filePath));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or CryptographicException)
        {
            LogEdgeCaCertificateNotReadable(logger, ex, filePath);
            return null;
        }
    }

    private static bool IsSignedBy(
        X509Certificate certificate,
        X509Certificate2 edgeCaCertificate)
    {
        // The same check as the device SDK's CustomCertificateValidator: build the chain with the edge CA, and require
        // that it ends at exactly that certificate.
        using var chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.ExtraStore.Add(edgeCaCertificate);
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

        using var serverCertificate = new X509Certificate2(certificate);
        return chain.Build(serverCertificate) &&
               chain.ChainElements[^1].Certificate.Equals(edgeCaCertificate);
    }

    [LoggerMessage(
        EventId = LoggingEventIdConstants.ServerCertificateValidationWouldFail,
        Level = LogLevel.Warning,
        Message = "Server certificate validation would have failed for '{Subject}': {Problem}. The connection is allowed, because the server certificate validation is Audit")]
    private static partial void LogServerCertificateValidationWouldFail(
        ILogger logger,
        string subject,
        string problem);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.EdgeCaCertificateNotReadable,
        Level = LogLevel.Warning,
        Message = "The edge CA certificate '{FilePath}' can't be read, so the audit can't check certificates against it")]
    private static partial void LogEdgeCaCertificateNotReadable(
        ILogger logger,
        Exception exception,
        string filePath);
}