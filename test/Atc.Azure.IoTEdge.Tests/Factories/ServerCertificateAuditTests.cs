namespace Atc.Azure.IoTEdge.Tests.Factories;

public sealed class ServerCertificateAuditTests : IDisposable
{
    private readonly TestCertificates certificates = new();
    private readonly FakeLogger logger = new();

    public void Dispose()
        => certificates.Dispose();

    [Fact]
    public void GetValidationProblem_NoErrors_ReturnsNull()
        => ServerCertificateAudit.GetValidationProblem(certificates.Server, chain: null, SslPolicyErrors.None, edgeCaCertificate: null)
            .Should().BeNull();

    [Fact]
    public void GetValidationProblem_NoCertificate_ReportsThatItIsMissing()
        => ServerCertificateAudit.GetValidationProblem(certificate: null, chain: null, SslPolicyErrors.RemoteCertificateNotAvailable, edgeCaCertificate: null)
            .Should().Be("RemoteCertificateNotAvailable");

    [Fact]
    public void GetValidationProblem_CertificateIsForAnotherName_ReportsTheNameMismatch()
        => ServerCertificateAudit.GetValidationProblem(certificates.Server, chain: null, SslPolicyErrors.RemoteCertificateNameMismatch, certificates.Ca)
            .Should().Be("RemoteCertificateNameMismatch");

    [Fact]
    public void GetValidationProblem_SignedByTheEdgeCa_ReturnsNull()
        => ServerCertificateAudit.GetValidationProblem(certificates.Server, chain: null, SslPolicyErrors.RemoteCertificateChainErrors, certificates.Ca)
            .Should().BeNull();

    [Fact]
    public void GetValidationProblem_SignedByAnotherCa_ReportsTheChainProblem()
    {
        // Arrange
        // For example a TLS-inspecting firewall that re-signs the traffic with its own CA.
        using var otherCertificates = new TestCertificates("Firewall CA");

        // Act
        var actual = ServerCertificateAudit.GetValidationProblem(otherCertificates.Server, chain: null, SslPolicyErrors.RemoteCertificateChainErrors, certificates.Ca);

        // Assert
        actual.Should().Be("RemoteCertificateChainErrors");
    }

    [Fact]
    public void GetValidationProblem_ChainErrorsWithoutAnEdgeCa_ReportsTheChainStatus()
    {
        // Arrange
        using var chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.Build(certificates.Server);

        // Act
        var actual = ServerCertificateAudit.GetValidationProblem(certificates.Server, chain, SslPolicyErrors.RemoteCertificateChainErrors, edgeCaCertificate: null);

        // Assert
        actual.Should().NotBeNullOrEmpty()
            .And.NotBe("RemoteCertificateChainErrors");
    }

    [Fact]
    public void CreateCallback_ValidationWouldFail_AcceptsTheCertificateAndLogsAWarning()
    {
        // Arrange
        var callback = ServerCertificateAudit.CreateCallback(logger, certificates.Ca);

        // Act
        var actual = callback(new object(), certificates.Server, chain: null, SslPolicyErrors.RemoteCertificateNameMismatch);

        // Assert
        actual.Should().BeTrue();
        logger.Collector.LatestRecord.Level.Should().Be(LogLevel.Warning);
        logger.Collector.LatestRecord.Message.Should().Be(
            "Server certificate validation would have failed for 'CN=edgehub': RemoteCertificateNameMismatch. " +
            "The connection is allowed, because the server certificate validation is Audit");
    }

    [Fact]
    public void CreateCallback_ValidationWouldSucceed_AcceptsTheCertificateWithoutLogging()
    {
        // Arrange
        var callback = ServerCertificateAudit.CreateCallback(logger, certificates.Ca);

        // Act
        var actual = callback(new object(), certificates.Server, chain: null, SslPolicyErrors.None);

        // Assert
        actual.Should().BeTrue();
        logger.Collector.Count.Should().Be(0);
    }

    [Fact]
    public void LoadEdgeCaCertificate_NoFileGiven_ReturnsNull()
        => ServerCertificateAudit.LoadEdgeCaCertificate(filePath: null, logger).Should().BeNull();

    [Fact]
    public void LoadEdgeCaCertificate_PemFile_ReturnsTheCertificate()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"atc-edge-ca-{Guid.NewGuid():N}.pem");
        File.WriteAllText(filePath, certificates.Ca.ExportCertificatePem());

        try
        {
            // Act
            using var actual = ServerCertificateAudit.LoadEdgeCaCertificate(filePath, logger);

            // Assert
            actual.Should().NotBeNull();
            actual!.Thumbprint.Should().Be(certificates.Ca.Thumbprint);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void LoadEdgeCaCertificate_FileDoesNotExist_ReturnsNullAndLogsAWarning()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"atc-edge-ca-{Guid.NewGuid():N}.pem");

        // Act
        var actual = ServerCertificateAudit.LoadEdgeCaCertificate(filePath, logger);

        // Assert
        actual.Should().BeNull();
        logger.Collector.LatestRecord.Level.Should().Be(LogLevel.Warning);
        logger.Collector.LatestRecord.Message.Should().Contain("can't be read");
    }
}