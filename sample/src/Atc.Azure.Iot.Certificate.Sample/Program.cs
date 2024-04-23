// CA1852 Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CS1587
#pragma warning disable CA1852

const string CertificateSubjectName = "opcpublisher";
const string OrganizationName = "myorganization";
using var cts = new CancellationTokenSource();

/// <summary>
/// Build the X509 Authority Key extension.
/// </summary>
/// <param name="issuerName">The distinguished name of the issuer.</param>
/// <param name="issuerSerialNumber">The serial number of the issuer.</param>
/// <param name="ski">The subject key identifier extension to use.</param>
#pragma warning restore CS1587
static X509Extension BuildAuthorityKeyIdentifier(
    X500DistinguishedName issuerName,
    byte[] issuerSerialNumber,
    X509SubjectKeyIdentifierExtension ski)
{
    Asn1Tag keyIdTag = new(TagClass.ContextSpecific, 0);
    Asn1Tag issuerNameTag = new(TagClass.ContextSpecific, 1);
    Asn1Tag issuerSerialTag = new(TagClass.ContextSpecific, 2);
    Asn1Tag directoryNameTag = new(TagClass.ContextSpecific, 4, isConstructed: true);

    var writer = new AsnWriter(AsnEncodingRules.DER);
    writer.PushSequence();

    if (ski?.SubjectKeyIdentifier is not null)
    {
        writer.WriteOctetString(
            HexToByteArray(ski.SubjectKeyIdentifier),
            keyIdTag);
    }

    writer.PushSequence(issuerNameTag);

    // Add the tag to constructed context-specific 4 (GeneralName.directoryName)
    writer.PushSetOf(directoryNameTag);
    var issuerNameRaw = issuerName.RawData;
    writer.WriteEncodedValue(issuerNameRaw);
    writer.PopSetOf(directoryNameTag);
    writer.PopSequence(issuerNameTag);

    var issuerSerial = new BigInteger(issuerSerialNumber);
    writer.WriteInteger(issuerSerial, issuerSerialTag);

    writer.PopSequence();

    return new X509Extension(
        oid: "2.5.29.35",
        rawData: writer.Encode(),
        critical: false);
}

static byte[] HexToByteArray(string hexString)
{
    var bytes = new byte[hexString.Length / 2];

    for (var i = 0; i < hexString.Length; i += 2)
    {
        var s = hexString.Substring(i, 2);
        bytes[i / 2] = byte.Parse(s, NumberStyles.HexNumber, null);
    }

    return bytes;
}

#pragma warning disable CS1587
/// <summary>
/// Creates a 20-byte cryptographic random serial number with the most significant bit of the first byte set to 0.
/// </summary>
/// <returns>A 20-byte array representing the serial number.</returns>
#pragma warning restore CS1587
static IEnumerable<byte> CreateSerialNumber()
{
    var serialNumber = new byte[20];
    RandomNumberGenerator.Fill(serialNumber);
    serialNumber[0] &= 0x7F;

    return serialNumber;
}

const int CertificatePathLength = 0;
const string subjectName = $"O={OrganizationName}, CN={CertificateSubjectName}";

// Generate RSA key pair
using var rsaPublicKey = RSA.Create(4096);

// Generate X500DistinguishedName
var subjectDistinguishedName = new X500DistinguishedName(subjectName);

// Create a certificate request
var request = new CertificateRequest(
    subjectDistinguishedName,
    rsaPublicKey,
    HashAlgorithmName.SHA256,
    RSASignaturePadding.Pkcs1);

// Set basic certificate constraints
request.CertificateExtensions.Add(
    new X509BasicConstraintsExtension(
        certificateAuthority: false,
        hasPathLengthConstraint: false,
        pathLengthConstraint: CertificatePathLength,
        critical: false));

// Set Subject Key Identifier
var ski = new X509SubjectKeyIdentifierExtension(
    request.PublicKey,
    X509SubjectKeyIdentifierHashAlgorithm.Sha1,
    critical: false);

request.CertificateExtensions.Add(ski);

var serialNumber = CreateSerialNumber();

request.CertificateExtensions.Add(
    BuildAuthorityKeyIdentifier(
        subjectDistinguishedName,
        serialNumber.Reverse().ToArray(),
        ski));

// Set Key usage
request.CertificateExtensions.Add(
    new X509KeyUsageExtension(
        keyUsages: X509KeyUsageFlags.DigitalSignature |
                   X509KeyUsageFlags.NonRepudiation |
                   X509KeyUsageFlags.KeyEncipherment |
                   X509KeyUsageFlags.DataEncipherment,
        critical: false));

// Set Subject Alternative Name
var sanBuilder = new SubjectAlternativeNameBuilder();
sanBuilder.AddUri(new Uri("urn:localhost:Microsoft.Azure.IIoT:microsoft"));
sanBuilder.AddDnsName("iotedge");
var sanExtension = sanBuilder.Build();
request.CertificateExtensions.Add(sanExtension);

// Set Enhanced key usages
request.CertificateExtensions.Add(
    new X509EnhancedKeyUsageExtension(
        [
            new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
        ],
        critical: false));

// Create self-signed certificate
var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(50));
var certBytes = certificate.Export(X509ContentType.Cert);

// Save the DER encoded certificate to disk
await File.WriteAllBytesAsync($"{CertificateSubjectName}.der", certBytes, cts.Token);

// Export as PFX
var pfxBytes = certificate.Export(X509ContentType.Pfx);
await File.WriteAllBytesAsync($"{CertificateSubjectName}.pfx", pfxBytes, cts.Token);