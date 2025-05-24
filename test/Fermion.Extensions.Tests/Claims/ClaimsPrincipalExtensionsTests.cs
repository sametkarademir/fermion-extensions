using System.Security.Claims;

namespace Fermion.Extensions.Claims;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserIdToString_WithValidClaim_ReturnsIdValue()
    {
        // Arrange
        const string userId = "user123";
        var principal = CreatePrincipalWithClaim(ClaimTypes.NameIdentifier, userId);

        // Act
        var result = principal.GetUserIdToString();

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserIdToString_WithoutClaim_ReturnsNull()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim("OtherClaimType", "value");

        // Act
        var result = principal.GetUserIdToString();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserIdToGuid_WithValidGuid_ReturnsGuid()
    {
        // Arrange
        var guidId = Guid.NewGuid();
        var principal = CreatePrincipalWithClaim(ClaimTypes.NameIdentifier, guidId.ToString());

        // Act
        var result = principal.GetUserIdToGuid();

        // Assert
        Assert.Equal(guidId, result);
    }

    [Fact]
    public void GetUserIdToGuid_WithInvalidGuidFormat_ReturnsNull()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim(ClaimTypes.NameIdentifier, "not-a-guid");

        // Act
        var result = principal.GetUserIdToGuid();

        // Assert
        Assert.Null((object?)result);
    }

    [Fact]
    public void GetUserIdToGuid_WithoutClaim_ReturnsNull()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim("OtherClaimType", "value");

        // Act
        var result = principal.GetUserIdToGuid();

        // Assert
        Assert.Null((object?)result);
    }

    [Fact]
    public void GetUserName_WithValidClaim_ReturnsNameValue()
    {
        // Arrange
        const string userName = "John Doe";
        var principal = CreatePrincipalWithClaim(ClaimTypes.Name, userName);

        // Act
        var result = principal.GetUserName();

        // Assert
        Assert.Equal(userName, result);
    }

    [Fact]
    public void GetUserName_WithoutClaim_ReturnsNull()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim("OtherClaimType", "value");

        // Act
        var result = principal.GetUserName();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserEmail_WithValidClaim_ReturnsEmailValue()
    {
        // Arrange
        const string email = "user@example.com";
        var principal = CreatePrincipalWithClaim(ClaimTypes.Email, email);

        // Act
        var result = principal.GetUserEmail();

        // Assert
        Assert.Equal(email, result);
    }

    [Fact]
    public void GetUserEmail_WithoutClaim_ReturnsNull()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim("OtherClaimType", "value");

        // Act
        var result = principal.GetUserEmail();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserRoles_WithMultipleRoleClaims_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<string> { "Admin", "User", "Manager" };
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        claims.Add(new Claim("OtherClaim", "value"));
        var principal = CreatePrincipalWithClaims(claims);

        // Act
        var result = principal.GetUserRoles();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roles.Count, result.Count);
        foreach (var role in roles)
        {
            Assert.Contains(role, result);
        }
    }

    [Fact]
    public void GetUserRoles_WithoutRoleClaims_ReturnsEmptyList()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim("OtherClaimType", "value");

        // Act
        var result = principal.GetUserRoles();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetUserCustomProperty_WithMatchingKey_ReturnsPropertyValue()
    {
        // Arrange
        const string key = "CustomProperty";
        const string value = "CustomValue";
        var principal = CreatePrincipalWithClaim(key, value);

        // Act
        var result = principal.GetUserCustomProperty(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void GetUserCustomProperty_WithNonMatchingKey_ReturnsNull()
    {
        // Arrange
        var principal = CreatePrincipalWithClaim("SomeKey", "SomeValue");

        // Act
        var result = principal.GetUserCustomProperty("OtherKey");

        // Assert
        Assert.Null(result);
    }

    private ClaimsPrincipal CreatePrincipalWithClaim(string claimType, string claimValue)
    {
        var claims = new List<Claim> { new Claim(claimType, claimValue) };
        return CreatePrincipalWithClaims(claims);
    }

    private ClaimsPrincipal CreatePrincipalWithClaims(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        return new ClaimsPrincipal(identity);
    }
}