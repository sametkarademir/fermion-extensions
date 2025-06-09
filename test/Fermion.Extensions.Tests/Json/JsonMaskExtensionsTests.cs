using System.Text.Json;

namespace Fermion.Extensions.Json;

public class JsonMaskExtensionsTests
{
    [Fact]
    public void MaskSensitiveData_WithNullOrEmptyInput_ShouldReturnSameInput()
    {
        // Arrange
        string? nullData = null;
        var emptyData = string.Empty;

        // Act
        var nullResult = JsonMaskExtensions.MaskSensitiveData(nullData);
        var emptyResult = JsonMaskExtensions.MaskSensitiveData(emptyData);

        // Assert
        Assert.Null(nullResult);
        Assert.Equal(string.Empty, emptyResult);
    }

    [Fact]
    public void MaskSensitiveData_WithSimpleJsonObject_ShouldMaskDefaultSensitiveProperties()
    {
        // Arrange
        const string jsonData = @"{
                ""Username"": ""john"",
                ""Password"": ""secret123"",
                ""Email"": ""john@example.com""
            }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.True(resultObj.TryGetProperty("Username", out var username));
        Assert.Equal("john", username.GetString());

        Assert.True(resultObj.TryGetProperty("Password", out var password));
        Assert.Equal("***MASKED***", password.GetString());

        Assert.True(resultObj.TryGetProperty("Email", out var email));
        Assert.Equal("john@example.com", email.GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithNestedJsonObject_ShouldMaskAllSensitiveProperties()
    {
        // Arrange
        const string jsonData = @"{
                ""User"": {
                    ""Username"": ""jane"",
                    ""Password"": ""password123"",
                    ""Token"": ""abcd1234""
                },
                ""ApiAccess"": {
                    ""Key"": ""api-key-12345"",
                    ""Endpoint"": ""https://api.example.com""
                }
            }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        var user = resultObj.GetProperty("User");
        Assert.Equal("jane", user.GetProperty("Username").GetString());
        Assert.Equal("***MASKED***", user.GetProperty("Password").GetString());
        Assert.Equal("***MASKED***", user.GetProperty("Token").GetString());

        var apiAccess = resultObj.GetProperty("ApiAccess");
        Assert.Equal("***MASKED***", apiAccess.GetProperty("Key").GetString());
        Assert.Equal("https://api.example.com", apiAccess.GetProperty("Endpoint").GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithJsonArray_ShouldMaskSensitivePropertiesInArrayItems()
    {
        // Arrange
        const string jsonData = @"[
                {
                    ""Username"": ""user1"",
                    ""Password"": ""pass1"",
                    ""Email"": ""user1@example.com""
                },
                {
                    ""Username"": ""user2"",
                    ""Password"": ""pass2"",
                    ""Email"": ""user2@example.com""
                }
            ]";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultArray = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.Equal(JsonValueKind.Array, resultArray.ValueKind);

        var firstItem = resultArray[0];
        Assert.Equal("user1", firstItem.GetProperty("Username").GetString());
        Assert.Equal("***MASKED***", firstItem.GetProperty("Password").GetString());
        Assert.Equal("user1@example.com", firstItem.GetProperty("Email").GetString());

        var secondItem = resultArray[1];
        Assert.Equal("user2", secondItem.GetProperty("Username").GetString());
        Assert.Equal("***MASKED***", secondItem.GetProperty("Password").GetString());
        Assert.Equal("user2@example.com", secondItem.GetProperty("Email").GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithCustomMaskPattern_ShouldUseCustomPattern()
    {
        // Arrange
        const string jsonData = @"{
                ""Username"": ""admin"",
                ""Password"": ""admin123"",
                ""Email"": ""admin@example.com""
            }";
        const string customMaskPattern = "[REDACTED]";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData, customMaskPattern);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.True(resultObj.TryGetProperty("Password", out var password));
        Assert.Equal("[REDACTED]", password.GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithCustomSensitiveProperties_ShouldMaskOnlySpecifiedProperties()
    {
        // Arrange
        const string jsonData = @"{
                ""Username"": ""john"",
                ""Password"": ""secret123"",
                ""Email"": ""john@example.com"",
                ""PhoneNumber"": ""123-456-7890""
            }";
        var sensitiveProps = new[] { "Email", "PhoneNumber" };

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData, sensitivePropertyNames: sensitiveProps);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.True(resultObj.TryGetProperty("Username", out var username));
        Assert.Equal("john", username.GetString());

        Assert.True(resultObj.TryGetProperty("Password", out var password));
        Assert.Equal("secret123", password.GetString()); // Not masked because not in our custom list

        Assert.True(resultObj.TryGetProperty("Email", out var email));
        Assert.Equal("***MASKED***", email.GetString());

        Assert.True(resultObj.TryGetProperty("PhoneNumber", out var phone));
        Assert.Equal("***MASKED***", phone.GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithAllValueTypes_ShouldPreserveTypes()
    {
        // Arrange
        const string jsonData = @"{
                ""StringValue"": ""text"",
                ""NumberValue"": 42,
                ""BoolValue"": true,
                ""NullValue"": null,
                ""DateValue"": ""2023-01-01T00:00:00Z"",
                ""ArrayValue"": [1, 2, 3],
                ""ObjectValue"": { ""Key"": ""value"" }
            }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.Equal(JsonValueKind.String, resultObj.GetProperty("StringValue").ValueKind);
        Assert.Equal(JsonValueKind.Number, resultObj.GetProperty("NumberValue").ValueKind);
        Assert.Equal(JsonValueKind.True, resultObj.GetProperty("BoolValue").ValueKind);
        Assert.Equal(JsonValueKind.Null, resultObj.GetProperty("NullValue").ValueKind);
        Assert.Equal(JsonValueKind.String, resultObj.GetProperty("DateValue").ValueKind);
        Assert.Equal(JsonValueKind.Array, resultObj.GetProperty("ArrayValue").ValueKind);
        Assert.Equal(JsonValueKind.Object, resultObj.GetProperty("ObjectValue").ValueKind);

        // And check the Key property is masked
        Assert.Equal("***MASKED***", resultObj.GetProperty("ObjectValue").GetProperty("Key").GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithInvalidJson_ShouldFallbackToRegexMasking()
    {
        // Arrange
        const string invalidJsonData = @"{
                ""Username"": ""john"",
                ""Password"": ""secret123"",
                This is invalid JSON
            }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(invalidJsonData);

        // Assert
        Assert.Contains("\"Password\": \"***MASKED***\"", result);
        Assert.Contains("Username", result);
        Assert.Contains("john", result);
    }

    [Fact]
    public void MaskSensitiveData_WithCaseSensitivity_ShouldIgnoreCase()
    {
        // Arrange
        const string jsonData = @"{
                ""username"": ""john"",
                ""PASSWORD"": ""secret123"",
                ""Token"": ""abcd1234"",
                ""secret"": ""hidden""
            }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.Equal("john", resultObj.GetProperty("username").GetString());
        Assert.Equal("***MASKED***", resultObj.GetProperty("PASSWORD").GetString());
        Assert.Equal("***MASKED***", resultObj.GetProperty("Token").GetString());
        Assert.Equal("***MASKED***", resultObj.GetProperty("secret").GetString());
    }

    [Fact]
    public void MaskSensitiveData_WithComplexNestedStructure_ShouldMaskAllSensitiveData()
    {
        // Arrange
        const string jsonData = @"{
                ""Users"": [
                    {
                        ""Profile"": {
                            ""Username"": ""user1"",
                            ""Password"": ""pass1""
                        },
                        ""Security"": {
                            ""ApiKey"": ""key1"",
                            ""Settings"": {
                                ""TwoFactor"": true,
                                ""RecoveryKey"": ""recovery1""
                            }
                        }
                    },
                    {
                        ""Profile"": {
                            ""Username"": ""user2"",
                            ""Password"": ""pass2""
                        },
                        ""Security"": {
                            ""ApiKey"": ""key2"",
                            ""Settings"": {
                                ""TwoFactor"": false,
                                ""RecoveryKey"": ""recovery2""
                            }
                        }
                    }
                ]
            }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        var users = resultObj.GetProperty("Users");

        var user1 = users[0];
        Assert.Equal("user1", user1.GetProperty("Profile").GetProperty("Username").GetString());
        Assert.Equal("***MASKED***", user1.GetProperty("Profile").GetProperty("Password").GetString());
        Assert.Equal("***MASKED***", user1.GetProperty("Security").GetProperty("ApiKey").GetString());
        Assert.True(user1.GetProperty("Security").GetProperty("Settings").GetProperty("TwoFactor").GetBoolean());
        Assert.Equal("***MASKED***",
            user1.GetProperty("Security").GetProperty("Settings").GetProperty("RecoveryKey").GetString());

        var user2 = users[1];
        Assert.Equal("user2", user2.GetProperty("Profile").GetProperty("Username").GetString());
        Assert.Equal("***MASKED***", user2.GetProperty("Profile").GetProperty("Password").GetString());
        Assert.Equal("***MASKED***", user2.GetProperty("Security").GetProperty("ApiKey").GetString());
        Assert.False(user2.GetProperty("Security").GetProperty("Settings").GetProperty("TwoFactor").GetBoolean());
        Assert.Equal("***MASKED***",
            user2.GetProperty("Security").GetProperty("Settings").GetProperty("RecoveryKey").GetString());
    }


    [Fact]
    public void MaskSensitiveData_ShouldMaskOnlySensitivePartsInConnectionString()
    {
        // Arrange
        const string jsonData = @"{
            ""ConnectionString"": ""Server=myserver;Database=mydb;User=myuser;Password=secret123;"",
            ""OtherConnectionString"": ""Server=otherserver;Database=otherdb;User=otheruser;Password=otherpass;"",
            ""RegularText"": ""This is normal text""
        }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.Equal("Server=myserver;Database=mydb;User=myuser;Password=***MASKED***;", resultObj.GetProperty("ConnectionString").GetString());
        Assert.Equal("Server=otherserver;Database=otherdb;User=otheruser;Password=***MASKED***;", resultObj.GetProperty("OtherConnectionString").GetString());
        Assert.Equal("This is normal text", resultObj.GetProperty("RegularText").GetString());
    }

    [Fact]
    public void MaskSensitiveData_ShouldMaskMultipleSensitivePartsInConnectionString()
    {
        // Arrange
        const string jsonData = @"{
            ""ConnectionString"": ""Server=myserver;Database=mydb;User=myuser;Password=secret123;ApiKey=abc123;Token=xyz789;"",
            ""RegularText"": ""This is normal text""
        }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.Equal("Server=myserver;Database=mydb;User=myuser;Password=***MASKED***;ApiKey=***MASKED***;Token=***MASKED***;", resultObj.GetProperty("ConnectionString").GetString());
        Assert.Equal("This is normal text", resultObj.GetProperty("RegularText").GetString());
    }

    [Fact]
    public void MaskSensitiveData_ShouldHandleConnectionStringInNestedObjects()
    {
        // Arrange
        const string jsonData = @"{
            ""Config"": {
                ""ConnectionString"": ""Server=myserver;Database=mydb;User=myuser;Password=secret123;"",
                ""OtherConnectionString"": ""Server=otherserver;Database=otherdb;User=otheruser;Password=otherpass;""
            },
            ""RegularText"": ""This is normal text""
        }";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultObj = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        var config = resultObj.GetProperty("Config");
        Assert.Equal("Server=myserver;Database=mydb;User=myuser;Password=***MASKED***;", config.GetProperty("ConnectionString").GetString());
        Assert.Equal("Server=otherserver;Database=otherdb;User=otheruser;Password=***MASKED***;", config.GetProperty("OtherConnectionString").GetString());
        Assert.Equal("This is normal text", resultObj.GetProperty("RegularText").GetString());
    }

    [Fact]
    public void MaskSensitiveData_ShouldHandleConnectionStringInArray()
    {
        // Arrange
        const string jsonData = @"[
            ""Server=server1;Database=db1;User=user1;Password=pass1;"",
            ""Server=server2;Database=db2;User=user2;Password=pass2;"",
            ""Regular text""
        ]";

        // Act
        var result = JsonMaskExtensions.MaskSensitiveData(jsonData);
        var resultArray = JsonSerializer.Deserialize<JsonElement>(result!);

        // Assert
        Assert.Equal("Server=server1;Database=db1;User=user1;Password=***MASKED***;", resultArray[0].GetString());
        Assert.Equal("Server=server2;Database=db2;User=user2;Password=***MASKED***;", resultArray[1].GetString());
        Assert.Equal("Regular text", resultArray[2].GetString());
    }
}