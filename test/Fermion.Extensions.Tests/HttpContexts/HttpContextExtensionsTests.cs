using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Fermion.Extensions.HttpContexts;

public class HttpContextExtensionsTests
{
    #region Headers Tests

    [Fact]
    public void GetRequestHeaderValue_ShouldReturnValue_WhenHeaderExists()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            { "Content-Type", new StringValues("application/json") }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetRequestHeaderValue("Content-Type");

        // Assert
        Assert.Equal("application/json", result);
    }

    [Fact]
    public void GetRequestHeaderValue_ShouldReturnNull_WhenHeaderDoesNotExist()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary();
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetRequestHeaderValue("Content-Type");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetRequestHeadersToDictionary_ShouldReturnAllHeaders()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            { "Content-Type", new StringValues("application/json") },
            { "Authorization", new StringValues("Bearer token123") }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetRequestHeadersToDictionary();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("application/json", result["Content-Type"]);
        Assert.Equal("Bearer token123", result["Authorization"]);
    }

    [Fact]
    public void GetRequestHeadersToJson_ShouldReturnValidJson()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            { "Content-Type", new StringValues("application/json") },
            { "Authorization", new StringValues("Bearer token123") }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetRequestHeadersToJson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("application/json", dictionary["Content-Type"]);
        Assert.Equal("Bearer token123", dictionary["Authorization"]);
    }

    [Fact]
    public void SetRequestHeaderValue_ShouldAddHeader()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary();
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        httpContext.SetRequestHeaderValue("X-Custom-Header", "CustomValue");

        // Assert
        Assert.True(headers.ContainsKey("X-Custom-Header"));
        Assert.Equal("CustomValue", headers["X-Custom-Header"].ToString());
    }

    [Fact]
    public void GetResponseHeadersToJson_ShouldReturnValidJson()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            { "Content-Type", new StringValues("application/json") },
            { "X-Response-Header", new StringValues("ResponseValue") }
        };
        Mock.Get(httpContext.Response).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetResponseHeadersToJson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("application/json", dictionary["Content-Type"]);
        Assert.Equal("ResponseValue", dictionary["X-Response-Header"]);
    }

    [Fact]
    public void SetResponseHeaderValue_ShouldAddHeader()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary();
        Mock.Get(httpContext.Response).Setup(r => r.Headers).Returns(headers);

        // Act
        httpContext.SetResponseHeaderValue("X-Custom-Header", "CustomValue");

        // Assert
        Assert.True(headers.ContainsKey("X-Custom-Header"));
        Assert.Equal("CustomValue", headers["X-Custom-Header"].ToString());
    }

    #endregion

    #region UserAgent Tests

    [Fact]
    public void GetUserAgent_ShouldReturnUserAgentString()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            {
                "User-Agent",
                new StringValues(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36")
            }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetUserAgent();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Mozilla", result);
    }

    [Fact]
    public void GetDeviceInfo_ShouldReturnDeviceInfo()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36";
        var headers = new HeaderDictionary
        {
            { "User-Agent", new StringValues(userAgent) }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetDeviceInfo();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Chrome", result.BrowserFamily);
        Assert.Equal("Windows", result.OsFamily);
    }

    [Fact]
    public void GetClientIpAddress_ShouldReturnIpFromXForwardedFor()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            { "X-Forwarded-For", new StringValues("192.168.1.1, 10.0.0.1") }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetClientIpAddress();

        // Assert
        Assert.Equal("192.168.1.1", result);
    }

    [Fact]
    public void GetClientIpAddress_ShouldReturnIpFromXRealIp_WhenXForwardedForIsNotPresent()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary
        {
            { "X-Real-IP", new StringValues("192.168.1.1") }
        };
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        // Act
        var result = httpContext.GetClientIpAddress();

        // Assert
        Assert.Equal("192.168.1.1", result);
    }

    [Fact]
    public void GetClientIpAddress_ShouldReturnRemoteIpAddress_WhenHeadersAreNotPresent()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary();
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        var mockConnection = new Mock<ConnectionInfo>();
        mockConnection.Setup(c => c.RemoteIpAddress).Returns(System.Net.IPAddress.Parse("192.168.1.1"));
        Mock.Get(httpContext).Setup(c => c.Connection).Returns(mockConnection.Object);

        // Act
        var result = httpContext.GetClientIpAddress();

        // Assert
        Assert.Equal("192.168.1.1", result);
    }

    [Fact]
    public void GetClientIpAddress_ShouldReturnUnknown_WhenNoIpIsAvailable()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary();
        Mock.Get(httpContext.Request).Setup(r => r.Headers).Returns(headers);

        var mockConnection = new Mock<ConnectionInfo>();
        mockConnection.Setup(c => c.RemoteIpAddress).Returns((System.Net.IPAddress)null!);
        Mock.Get(httpContext).Setup(c => c.Connection).Returns(mockConnection.Object);

        // Act
        var result = httpContext.GetClientIpAddress();

        // Assert
        Assert.Equal("unknown", result);
    }

    #endregion

    #region RequestPath Tests

    [Fact]
    public void GetBaseUrl_ShouldReturnSchemeAndHost()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        Mock.Get(httpContext.Request).Setup(r => r.Scheme).Returns("https");
        Mock.Get(httpContext.Request).Setup(r => r.Host).Returns(new HostString("example.com"));

        // Act
        var result = httpContext.GetBaseUrl();

        // Assert
        Assert.Equal("https://example.com", result);
    }

    [Fact]
    public void GetPath_ShouldReturnRequestPath()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        Mock.Get(httpContext.Request).Setup(r => r.Path).Returns(new PathString("/api/products"));

        // Act
        var result = httpContext.GetPath();

        // Assert
        Assert.Equal("/api/products", result);
    }

    [Fact]
    public void GetRequestMethod_ShouldReturnHttpMethod()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        Mock.Get(httpContext.Request).Setup(r => r.Method).Returns("GET");

        // Act
        var result = httpContext.GetRequestMethod();

        // Assert
        Assert.Equal("GET", result);
    }

    #endregion

    #region QueryString Tests

    [Fact]
    public void GetQueryStringValue_ShouldReturnValue_WhenParameterExists()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "id", new StringValues("123") }
        });
        Mock.Get(httpContext.Request).Setup(r => r.Query).Returns(queryCollection);

        // Act
        var result = httpContext.GetQueryStringValue("id");

        // Assert
        Assert.Equal("123", result);
    }

    [Fact]
    public void GetQueryStringValue_ShouldReturnNull_WhenParameterDoesNotExist()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var queryCollection = new QueryCollection(new Dictionary<string, StringValues>());
        Mock.Get(httpContext.Request).Setup(r => r.Query).Returns(queryCollection);

        // Act
        var result = httpContext.GetQueryStringValue("id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetQueryStringToDictionary_ShouldReturnAllParameters()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "id", new StringValues("123") },
            { "name", new StringValues("test") }
        });
        Mock.Get(httpContext.Request).Setup(r => r.Query).Returns(queryCollection);

        // Act
        var result = httpContext.GetQueryStringToDictionary();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("123", result["id"]);
        Assert.Equal("test", result["name"]);
    }

    [Fact]
    public void GetQueryStringToJson_ShouldReturnValidJson()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "id", new StringValues("123") },
            { "name", new StringValues("test") }
        });
        Mock.Get(httpContext.Request).Setup(r => r.Query).Returns(queryCollection);

        // Act
        var result = httpContext.GetQueryStringToJson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("123", dictionary["id"]);
        Assert.Equal("test", dictionary["name"]);
    }

    #endregion

    #region RequestBody Tests

    [Fact]
    public void GetRequestBody_ShouldReturnBody_WhenBodyCanSeek()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var body = "This is the request body";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        Mock.Get(httpContext.Request).Setup(r => r.Body).Returns(stream);

        // Act
        var result = httpContext.GetRequestBody();

        // Assert
        Assert.Equal(body, result);
    }

    [Fact]
    public void GetRequestBody_ShouldTruncateBody_WhenExceedsMaxLength()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var body = new string('a', 1050); // Create a string longer than the default maxLength (1000)
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        Mock.Get(httpContext.Request).Setup(r => r.Body).Returns(stream);

        // Act
        var result = httpContext.GetRequestBody();

        // Assert
        Assert.Contains("... (truncated)", result);
        Assert.True(result.Length <= 1000 + "... (truncated)".Length);
    }

    [Fact]
    public void GetRequestBodyToJson_ShouldReturnValidJson()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var body = "This is the request body";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        Mock.Get(httpContext.Request).Setup(r => r.Body).Returns(stream);

        // Act
        var result = httpContext.GetRequestBodyToJson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var deserializedValue = JsonSerializer.Deserialize<string>(result);
        Assert.NotNull(deserializedValue);
        Assert.Equal(body, deserializedValue);
    }

    #endregion

    #region Form Data Tests

    [Fact]
    public async Task GetFormValueAsync_ShouldReturnValue_WhenFieldExists()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var formCollection = new FormCollection(new Dictionary<string, StringValues>
        {
            { "name", new StringValues("John") }
        });

        Mock.Get(httpContext.Request).Setup(r => r.ReadFormAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(formCollection);

        // Act
        var result = await httpContext.GetFormValueAsync("name");

        // Assert
        Assert.Equal("John", result);
    }

    [Fact]
    public async Task GetFormValueAsync_ShouldReturnNull_WhenFieldDoesNotExist()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var formCollection = new FormCollection(new Dictionary<string, StringValues>());

        Mock.Get(httpContext.Request).Setup(r => r.ReadFormAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(formCollection);

        // Act
        var result = await httpContext.GetFormValueAsync("name");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFormDataToDictionaryAsync_ShouldReturnAllFields()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var formCollection = new FormCollection(new Dictionary<string, StringValues>
        {
            { "name", new StringValues("John") },
            { "age", new StringValues("30") }
        });

        Mock.Get(httpContext.Request).Setup(r => r.ReadFormAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(formCollection);

        // Act
        var result = await httpContext.GetFormDataToDictionaryAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("John", result["name"]);
        Assert.Equal("30", result["age"]);
    }

    [Fact]
    public async Task GetFormFileAsync_ShouldReturnNull_WhenFileDoesNotExist()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var formFiles = new FormFileCollection();
        var formCollection = new FormCollection(
            new Dictionary<string, StringValues>(),
            formFiles
        );

        Mock.Get(httpContext.Request).Setup(r => r.ReadFormAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(formCollection);

        // Act
        var result = await httpContext.GetFormFileAsync("profilePicture");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Helper Methods and Classes

    private HttpContext CreateMockHttpContext()
    {
        var mockHttpContext = new Mock<HttpContext>();
        var mockRequest = new Mock<HttpRequest>();
        var mockResponse = new Mock<HttpResponse>();

        mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

        return mockHttpContext.Object;
    }

    #endregion
}