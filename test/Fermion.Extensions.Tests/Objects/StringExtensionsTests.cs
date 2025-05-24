namespace Fermion.Extensions.Objects;

public class StringExtensionsTests
{
    [Fact]
    public void Replace_WithTurkishCharacters_ReturnsCorrectResult()
    {
        // Arrange
        const string input = "İıĞğÖöÜüŞşÇç";

        // Act
        var result = StringExtensions.Replace(input);

        // Assert
        Assert.Equal("IiGgOoUuSsCc", result);
    }

    [Fact]
    public void Replace_WithSpaces_ReplacesWithUnderscores()
    {
        // Arrange
        const string input = "Hello World";

        // Act
        var result = StringExtensions.Replace(input);

        // Assert
        Assert.Equal("Hello_World", result);
    }

    [Fact]
    public void GenerateBase64RandomId_ReturnsNonEmptyString()
    {
        // Act
        var result = StringExtensions.GenerateBase64RandomId();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(8, result.Length); // Based on the implementation, should be 8 characters
    }

    [Fact]
    public void GenerateBase64RandomId_ReturnsDifferentValuesOnMultipleCalls()
    {
        // Act
        var result1 = StringExtensions.GenerateBase64RandomId();
        var result2 = StringExtensions.GenerateBase64RandomId();

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void ToSlug_WithValidInput_ReturnsCorrectSlug()
    {
        // Arrange
        const string input = "Hello World! This is a test.";

        // Act
        var result = input.ToSlug();

        // Assert
        Assert.Equal("hello-world-this-is-a-test", result);
    }

    [Fact]
    public void ToSlug_WithTurkishCharacters_ReturnsCorrectSlug()
    {
        // Arrange
        const string input = "Merhaba Dünya! Bu bir Türkçe testtir.";

        // Act
        var result = input.ToSlug();

        // Assert
        Assert.Equal("merhaba-dunya-bu-bir-turkce-testtir", result);
    }

    [Fact]
    public void ToSlug_WithNullOrEmpty_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, ((string)null!).ToSlug());
        Assert.Equal(string.Empty, string.Empty.ToSlug());
        Assert.Equal(string.Empty, "   ".ToSlug());
    }

    [Fact]
    public void Truncate_WithShortInput_ReturnsOriginalString()
    {
        // Arrange
        const string input = "Hello";

        // Act
        var result = input.Truncate(10);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Truncate_WithLongInput_ReturnsTruncatedString()
    {
        // Arrange
        const string input = "Hello, this is a long text that needs to be truncated.";

        // Act
        var result = input.Truncate(10);

        // Assert
        Assert.Equal("Hello, thi...", result);
    }

    [Fact]
    public void Truncate_WithCustomSuffix_ReturnsTruncatedStringWithCustomSuffix()
    {
        // Arrange
        const string input = "Hello, this is a long text that needs to be truncated.";

        // Act
        var result = input.Truncate(10, " [MORE]");

        // Assert
        Assert.Equal("Hello, thi [MORE]", result);
    }

    [Fact]
    public void RemoveHtmlTags_WithHtmlInput_RemovesTags()
    {
        // Arrange
        const string input = "<p>This is a <strong>test</strong> with <a href='#'>HTML</a> tags.</p>";

        // Act
        var result = input.RemoveHtmlTags();

        // Assert
        Assert.Equal("This is a test with HTML tags.", result);
    }

    [Fact]
    public void RemoveHtmlTags_WithNbsp_RemovesNbsp()
    {
        // Arrange
        const string input = "Text with&nbsp;non-breaking&nbsp;spaces.";

        // Act
        var result = input.RemoveHtmlTags(" ");

        // Assert
        Assert.Equal("Text with non-breaking spaces.", result);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name+tag@example.co.uk", true)]
    [InlineData("user@subdomain.example.com", true)]
    [InlineData("invalid@", false)]
    [InlineData("invalid@domain", true)]
    [InlineData("@domain.com", false)]
    [InlineData("plaintext", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidEmail_WithVariousInputs_ReturnsExpectedResults(string email, bool expected)
    {
        // Act
        var result = email.IsValidEmail();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://www.example.com", true)]
    [InlineData("http://example.com", true)]
    [InlineData("https://subdomain.example.co.uk/path?query=1", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("www.example.com", false)] // Missing schema
    [InlineData("example.com", false)] // Missing schema
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidUrl_WithVariousInputs_ReturnsExpectedResults(string url, bool expected)
    {
        // Act
        var result = url.IsValidUrl();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBase64_WithValidInput_EncodesCorrectly()
    {
        // Arrange
        const string input = "Hello, World!";
        const string expected = "SGVsbG8sIFdvcmxkIQ==";

        // Act
        var result = input.ToBase64();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBase64_WithEmptyInput_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, string.Empty.ToBase64());
        Assert.Equal(string.Empty, ((string)null!).ToBase64());
    }

    [Fact]
    public void FromBase64_WithValidInput_DecodesCorrectly()
    {
        // Arrange
        const string base64 = "SGVsbG8sIFdvcmxkIQ==";
        const string expected = "Hello, World!";

        // Act
        var result = base64.FromBase64();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FromBase64_WithInvalidInput_ReturnsEmptyString()
    {
        // Arrange
        const string invalidBase64 = "ThisIsNotBase64!";

        // Act
        var result = invalidBase64.FromBase64();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FromBase64_WithEmptyInput_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, string.Empty.FromBase64());
        Assert.Equal(string.Empty, ((string)null!).FromBase64());
    }

    [Fact]
    public void ToMd5Hash_WithValidInput_ReturnsCorrectHash()
    {
        // Arrange
        const string input = "Hello, World!";
        const string expected = "65a8e27d8879283831b664bd8b7f0ad4";

        // Act
        var result = input.ToMd5Hash();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToMd5Hash_WithEmptyInput_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, string.Empty.ToMd5Hash());
        Assert.Equal(string.Empty, ((string)null!).ToMd5Hash());
    }

    [Fact]
    public void ToSha256Hash_WithValidInput_ReturnsCorrectHash()
    {
        // Arrange
        const string input = "Hello, World!";
        const string expected = "dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f";

        // Act
        var result = input.ToSha256Hash();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToSha256Hash_WithEmptyInput_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, string.Empty.ToSha256Hash());
        Assert.Equal(string.Empty, ((string)null!).ToSha256Hash());
    }

    [Theory]
    [InlineData("Hello World", "hello", true)]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "universe", false)]
    [InlineData("", "test", false)]
    [InlineData("Hello World", "", false)]
    [InlineData(null, "test", false)]
    public void ContainsIgnoreCase_WithVariousInputs_ReturnsExpectedResults(string source, string value, bool expected)
    {
        // Act
        var result = source.ContainsIgnoreCase(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToTitleCase_WithValidInput_ReturnsCorrectTitleCase()
    {
        // Arrange
        const string input = "hello world example";

        // Act
        var result = input.ToTitleCase();

        // Assert
        Assert.Equal("Hello World Example", result);
    }

    [Fact]
    public void ToTitleCase_WithMixedCaseInput_NormalizesAndReturnsCorrectTitleCase()
    {
        // Arrange
        const string input = "hElLo WoRlD eXaMpLe";

        // Act
        var result = input.ToTitleCase();

        // Assert
        Assert.Equal("Hello World Example", result);
    }

    [Fact]
    public void ToTitleCase_WithEmptyInput_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, string.Empty.ToTitleCase());
        Assert.Equal(string.Empty, ((string)null!).ToTitleCase());
    }

    [Fact]
    public void StripNonAlphanumeric_WithMixedInput_RemovesNonAlphanumericCharacters()
    {
        // Arrange
        const string input = "Hello, World! 123 #$%";

        // Act
        var result = input.StripNonAlphanumeric();

        // Assert
        Assert.Equal("HelloWorld123", result);
    }

    [Fact]
    public void StripNonAlphanumeric_WithAllowSpace_PreservesSpaces()
    {
        // Arrange
        const string input = "Hello, World! 123 #$%";

        // Act
        var result = input.StripNonAlphanumeric(true);

        // Assert
        Assert.Equal("Hello World 123 ", result);
    }

    [Fact]
    public void StripNonAlphanumeric_WithEmptyInput_ReturnsEmptyString()
    {
        // Act & Assert
        Assert.Equal(string.Empty, string.Empty.StripNonAlphanumeric());
        Assert.Equal(string.Empty, ((string)null!).StripNonAlphanumeric());
    }

    [Fact]
    public void SplitInParts_WithValidInput_SplitsCorrectly()
    {
        // Arrange
        const string input = "ABCDEFGHIJKL";

        // Act
        var parts = input.SplitInParts(4).ToList();

        // Assert
        Assert.Equal(3, parts.Count);
        Assert.Equal("ABCD", parts[0]);
        Assert.Equal("EFGH", parts[1]);
        Assert.Equal("IJKL", parts[2]);
    }

    [Fact]
    public void SplitInParts_WithInputNotDivisibleByPartLength_HandlesRemainder()
    {
        // Arrange
        const string input = "ABCDEFGHIJK"; // 11 chars

        // Act
        var parts = input.SplitInParts(4).ToList();

        // Assert
        Assert.Equal(3, parts.Count);
        Assert.Equal("ABCD", parts[0]);
        Assert.Equal("EFGH", parts[1]);
        Assert.Equal("IJK", parts[2]); // Remaining 3 chars
    }

    [Fact]
    public void SplitInParts_WithZeroOrNegativePartLength_ThrowsException()
    {
        // Arrange
        const string input = "ABCDEFG";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => input.SplitInParts(0).ToList());
        Assert.Throws<ArgumentException>(() => input.SplitInParts(-1).ToList());
    }
}