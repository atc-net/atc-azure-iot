namespace Atc.Azure.IoT.Wpf.App.Extensions;

/// <summary>
/// Provides extension methods for working with Claims.
/// </summary>
public static class ClaimsExtensions
{
    /// <summary>
    /// Gets the display name from a collection of claims.
    /// </summary>
    /// <param name="claims">The collection of claims.</param>
    /// <returns>The display name, or null if not found.</returns>
    public static string? GetDisplayName(
        this IEnumerable<Claim> claims)
    {
        return claims
            .FirstOrDefault(x => "name".Equals(x.Type, StringComparison.Ordinal))?
            .Value;
    }

    /// <summary>
    /// Gets the userName from a collection of claims.
    /// </summary>
    /// <param name="claims">The collection of claims.</param>
    /// <returns>The userName, or null if not found.</returns>
    public static string? GetUserName(
        this IEnumerable<Claim> claims)
    {
        return claims
            .FirstOrDefault(x => ClaimTypes.Name.Equals(x.Type, StringComparison.Ordinal))?
            .Value;
    }

    /// <summary>
    /// Gets the email from a collection of claims.
    /// </summary>
    /// <param name="claims">The collection of claims.</param>
    /// <returns>The email, or null if not found.</returns>
    public static string? GetEmail(
        this IEnumerable<Claim> claims)
    {
        var claimsList = claims.ToList();

        var email = claimsList
            .Find(x => ClaimTypes.Email.Equals(x.Type, StringComparison.Ordinal))?
            .Value;

        if (!string.IsNullOrEmpty(email))
        {
            return email;
        }

        email = claimsList
            .Find(x => "email".Equals(x.Type, StringComparison.Ordinal))?
            .Value;

        if (!string.IsNullOrEmpty(email))
        {
            return email;
        }

        var userName = claimsList.GetUserName();
        if (!string.IsNullOrEmpty(userName) && userName.IsEmailAddress())
        {
            email = userName;
        }

        return email;
    }
}