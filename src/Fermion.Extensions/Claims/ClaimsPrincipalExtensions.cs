using System.Security.Claims;

namespace Fermion.Extensions.Claims;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the claims principal as a string.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>The user ID as a string, or null if not found.</returns>
    /// <remarks>
    /// This method retrieves the user ID from the claims principal using the <see cref="ClaimTypes.NameIdentifier"/> claim type.
    /// If the claim is not found, it returns null.
    /// </remarks>
    public static string? GetUserIdToString(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim?.Value;
    }

    /// <summary>
    /// Gets the user ID from the claims principal as a GUID.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>The user ID as a GUID, or null if not found or invalid.</returns>
    /// <remarks>
    /// This method retrieves the user ID from the claims principal using the <see cref="ClaimTypes.NameIdentifier"/> claim type.
    /// If the claim is not found or cannot be parsed as a GUID, it returns null.
    /// </remarks>
    public static Guid? GetUserIdToGuid(this ClaimsPrincipal user)
    {
        var id = user.GetUserIdToString();
        if (id == null)
        {
            return null;
        }

        if (Guid.TryParse(id, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gets the username from the claims principal.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>The username as a string, or null if not found.</returns>
    /// <remarks>
    /// This method retrieves the username from the claims principal using the <see cref="ClaimTypes.Name"/> claim type.
    /// If the claim is not found, it returns null.
    /// </remarks>
    public static string? GetUserName(this ClaimsPrincipal user)
    {
        var userNameClaim = user.FindFirst(ClaimTypes.Name);
        return userNameClaim?.Value;
    }

    /// <summary>
    /// Gets the email address from the claims principal.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>The email address as a string, or null if not found.</returns>
    /// <remarks>
    /// This method retrieves the email address from the claims principal using the <see cref="ClaimTypes.Email"/> claim type.
    /// If the claim is not found, it returns null.
    /// </remarks>
    public static string? GetUserEmail(this ClaimsPrincipal user)
    {
        var userEmailClaim = user.FindFirst(ClaimTypes.Email);
        return userEmailClaim?.Value;
    }
    /// <summary>
    /// Gets the user roles from the claims principal.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>A list of role names as strings, or null if no roles are found.</returns>
    /// <remarks>
    /// This method retrieves all role claims from the claims principal using the <see cref="ClaimTypes.Role"/> claim type.
    /// If no role claims are found, it returns null.
    /// </remarks>

    public static List<string>? GetUserRoles(this ClaimsPrincipal user)
    {
        var userRolesClaim = user.FindAll(ClaimTypes.Role);
        return userRolesClaim?.Select(item => item.Value).ToList();
    }

    /// <summary>
    /// Gets a custom property from the claims principal.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <param name="key">The key of the custom property to retrieve.</param>
    /// <returns>The value of the custom property as a string, or null if not found.</returns>
    /// <remarks>
    /// This method retrieves a custom property from the claims principal using the specified key.
    /// If the claim is not found, it returns null.
    /// </remarks>
    public static string? GetUserCustomProperty(this ClaimsPrincipal user, string key)
    {
        var userCustomPropertyClaim = user.FindFirst(key);
        return userCustomPropertyClaim?.Value;
    }
}