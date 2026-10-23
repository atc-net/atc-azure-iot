namespace Atc.Azure.IoT.Options;

/// <summary>
/// Redacts the secrets in Azure IoT connection strings, so they can be logged.
/// </summary>
internal static class ConnectionStringRedactor
{
    private const string RedactedValue = "***";

    private static readonly string[] SecretKeys = ["SharedAccessKey", "SharedAccessSignature"];

    /// <summary>
    /// Replaces the values of the secret parts of a connection string, e.g. <c>SharedAccessKey</c>, with <c>***</c>.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The connection string with its secrets redacted.</returns>
    public static string Redact(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return string.Empty;
        }

        var parts = connectionString.Split(';');
        for (var i = 0; i < parts.Length; i++)
        {
            var separatorIndex = parts[i].IndexOf('=', StringComparison.Ordinal);
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = parts[i][..separatorIndex].Trim();
            if (SecretKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
            {
                parts[i] = $"{key}={RedactedValue}";
            }
        }

        return string.Join(';', parts);
    }
}