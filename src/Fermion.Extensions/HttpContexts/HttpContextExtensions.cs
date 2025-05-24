using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UAParser;

namespace Fermion.Extensions.HttpContexts;

/// <summary>
/// Provides extension methods for the HttpContext class to simplify common operations.
/// </summary>
public static class HttpContextExtensions
{
    #region Headers

    /// <summary>
    /// Gets a value from the request headers.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The header key.</param>
    /// <returns>The value of the specified header or null if not found.</returns>
    /// <example>
    /// <code>
    /// string contentType = httpContext.GetRequestHeaderValue("Content-Type");
    /// </code>
    /// </example>
    public static string? GetRequestHeaderValue(this HttpContext context, string key)
    {
        return context.Request.Headers.TryGetValue(key, out var values)
            ? values.FirstOrDefault()
            : null;
    }

    /// <summary>
    /// Converts all request headers to a dictionary.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A dictionary containing all request headers.</returns>
    /// <example>
    /// <code>
    /// var headers = httpContext.GetRequestHeadersToDictionary();
    /// foreach (var header in headers)
    /// {
    ///     Console.WriteLine($"{header.Key}: {header.Value}");
    /// }
    /// </code>
    /// </example>
    public static Dictionary<string, string> GetRequestHeadersToDictionary(this HttpContext context)
    {
        var headers = new Dictionary<string, string>();
        foreach (var header in context.Request.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }

        return headers;
    }

    /// <summary>
    /// Converts all request headers to a JSON string.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A JSON string containing all request headers.</returns>
    /// <example>
    /// <code>
    /// string headersJson = httpContext.GetRequestHeadersToJson();
    /// </code>
    /// </example>
    public static string GetRequestHeadersToJson(this HttpContext context)
    {
        return JsonSerializer.Serialize(context.GetRequestHeadersToDictionary(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Sets a value in the request headers.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <example>
    /// <code>
    /// httpContext.SetRequestHeaderValue("X-Custom-Header", "CustomValue");
    /// </code>
    /// </example>
    public static void SetRequestHeaderValue(this HttpContext context, string key, string value)
    {
        context.Request.Headers.Append(key, value);
    }

    /// <summary>
    /// Converts all response headers to a JSON string.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A JSON string containing all response headers.</returns>
    /// <example>
    /// <code>
    /// string responseHeadersJson = httpContext.GetResponseHeadersToJson();
    /// </code>
    /// </example>
    public static string GetResponseHeadersToJson(this HttpContext context)
    {
        var headers = new Dictionary<string, string>();
        foreach (var header in context.Response.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }

        return JsonSerializer.Serialize(headers, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Sets a value in the response headers.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <example>
    /// <code>
    /// httpContext.SetResponseHeaderValue("X-Custom-Header", "CustomValue");
    /// </code>
    /// </example>
    public static void SetResponseHeaderValue(this HttpContext context, string key, string value)
    {
        context.Response.Headers.Append(key, value);
    }

    /// <summary>
    /// Gets the correlation ID from the request headers
    /// </summary>
    /// <remarks>
    /// This method checks the "X-Correlation-ID" header for a correlation ID.
    /// If the header is not present or the value is not a valid GUID, it returns a new GUID.
    /// </remarks>
    /// <param name="httpContext"></param>
    /// <returns>
    /// A nullable GUID representing the correlation ID.
    /// If the header is not present or the value is not a valid GUID, it returns null.
    /// </returns>
    public static Guid? GetCorrelationId(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            return Guid.TryParse(correlationId, out var guid) ? guid : Guid.NewGuid();
        }

        return null;
    }

    /// <summary>
    /// Sets the correlation ID in the request headers
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="correlationId"></param>
    /// <remarks>
    /// This method sets the correlation ID in the request headers.
    /// It is used to track the request across different services.
    /// </remarks>
    public static void SetCorrelationId(this HttpContext httpContext, Guid correlationId)
    {
        httpContext.Request.Headers.Append("X-Correlation-ID", correlationId.ToString());
    }

    /// <summary>
    /// Gets the session ID from the request headers
    /// </summary>
    /// <remarks>
    /// This method checks the "X-Session-ID" header for a session ID.
    /// If the header is not present or the value is not a valid GUID, it returns null.
    /// </remarks>
    /// <param name="httpContext"></param>
    /// <returns>
    /// A nullable GUID representing the session ID.
    /// If the header is not present or the value is not a valid GUID, it returns null.
    /// </returns>
    public static Guid? GetSessionId(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Session-ID", out var sessionId))
        {
            return Guid.TryParse(sessionId, out var guid) ? guid : null;
        }

        return null;
    }

    /// <summary>
    /// Sets the session ID in the request headers
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="sessionId"></param>
    /// <remarks>
    /// This method sets the session ID in the request headers.
    /// It is used to track the session across different services.
    /// </remarks>
    public static void SetSessionId(this HttpContext httpContext, Guid sessionId)
    {
        httpContext.Request.Headers.Append("X-Session-ID", sessionId.ToString());
    }

    /// <summary>
    /// Gets the snapshot ID from the request headers
    /// </summary>
    /// <remarks>
    /// This method checks the "X-Snapshot-ID" header for a snapshot ID.
    /// If the header is not present or the value is not a valid GUID, it returns null.
    /// </remarks>
    /// <param name="httpContext"></param>
    /// <returns>
    /// A nullable GUID representing the snapshot ID.
    /// If the header is not present or the value is not a valid GUID, it returns null.
    /// </returns>
    public static Guid? GetSnapshotId(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Snapshot-ID", out var snapshotId))
        {
            return Guid.TryParse(snapshotId, out var guid) ? guid : null;
        }

        return null;
    }

    /// <summary>
    /// Sets the snapshot ID in the request headers
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="snapshotId"></param>
    /// <remarks>
    /// This method sets the snapshot ID in the request headers.
    /// It is used to track the snapshot across different services.
    /// </remarks>
    public static void SetSnapshotId(this HttpContext httpContext, Guid snapshotId)
    {
        httpContext.Request.Headers.Append("X-Snapshot-ID", snapshotId.ToString());
    }

    #endregion

    #region UserAgent

    /// <summary>
    /// Gets the User-Agent string from the request.
    /// </summary>
    /// <param name="httpContext">The HttpContext instance.</param>
    /// <returns>The User-Agent string.</returns>
    /// <example>
    /// <code>
    /// string userAgent = httpContext.GetUserAgent();
    /// </code>
    /// </example>
    public static string GetUserAgent(this HttpContext httpContext)
    {
        return httpContext.Request.Headers["User-Agent"].ToString();
    }

    /// <summary>
    /// Gets detailed device information from the User-Agent.
    /// </summary>
    /// <param name="httpContext">The HttpContext instance.</param>
    /// <returns>A DeviceInfo object containing detailed device information.</returns>
    /// <remarks>
    /// This method uses the UAParser library to parse the User-Agent string.
    /// </remarks>
    /// <example>
    /// <code>
    /// var deviceInfo = httpContext.GetDeviceInfo();
    /// Console.WriteLine($"Device: {deviceInfo.DeviceFamily}, Browser: {deviceInfo.BrowserFamily}");
    /// </code>
    /// </example>
    public static DeviceInfo GetDeviceInfo(this HttpContext httpContext)
    {
        var parser = Parser.GetDefault();
        var clientInfo = parser.Parse(httpContext.GetUserAgent());

        var deviceFamily = new DeviceInfo
        {
            DeviceFamily = clientInfo.Device.Family,
            DeviceModel = clientInfo.Device.Model,
            OsFamily = clientInfo.OS.Family,
            OsVersion = string.Join(".", new[]
            {
                clientInfo.OS.Major,
                clientInfo.OS.Minor,
                clientInfo.OS.Patch
            }.Where(v => !string.IsNullOrEmpty(v))),
            BrowserFamily = clientInfo.UA.Family,
            BrowserVersion = string.Join(".", new[]
            {
                clientInfo.UA.Major,
                clientInfo.UA.Minor
            }.Where(v => !string.IsNullOrEmpty(v)))
        };

        return deviceFamily;
    }

    /// <summary>
    /// Gets the client IP address, handling various proxy scenarios.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The client IP address as a string, or "unknown" if not available.</returns>
    /// <remarks>
    /// This method checks various headers commonly set by proxies (X-Forwarded-For, X-Real-IP)
    /// before falling back to the connection's remote IP address.
    /// </remarks>
    /// <example>
    /// <code>
    /// string clientIp = httpContext.GetClientIpAddress();
    /// </code>
    /// </example>
    public static string GetClientIpAddress(this HttpContext context)
    {
        string? ip = null;

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            ip = forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }

        if (string.IsNullOrEmpty(ip) && context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            ip = realIp.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }

        return ip ?? "unknown";
    }

    #endregion

    #region RequestPath

    /// <summary>
    /// Gets the base URL of the current request.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The base URL (scheme and host) of the current request.</returns>
    /// <example>
    /// <code>
    /// string baseUrl = httpContext.GetBaseUrl(); // e.g., "https://example.com"
    /// </code>
    /// </example>
    public static string GetBaseUrl(this HttpContext context)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}";
    }

    /// <summary>
    /// Gets the path of the current request.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The path of the current request.</returns>
    /// <example>
    /// <code>
    /// string path = httpContext.GetPath(); // e.g., "/api/products"
    /// </code>
    /// </example>
    public static string GetPath(this HttpContext context)
    {
        return context.Request.Path.ToString();
    }

    /// <summary>
    /// Gets the HTTP method of the current request.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The HTTP method (GET, POST, etc.) of the current request.</returns>
    /// <example>
    /// <code>
    /// string method = httpContext.GetRequestMethod(); // e.g., "GET" or "POST"
    /// </code>
    /// </example>
    public static string GetRequestMethod(this HttpContext context)
    {
        return context.Request.Method;
    }

    /// <summary>
    /// Gets the controller name from the route data.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The controller name, or "unknown" if not available.</returns>
    /// <example>
    /// <code>
    /// string controller = httpContext.GetControllerName();
    /// </code>
    /// </example>
    public static string? GetControllerName(this HttpContext context)
    {
        var routeValues = GetRouteValue(context);
        return routeValues.TryGetValue("controller", out var value) ? value?.ToString() : "unknown";
    }

    /// <summary>
    /// Gets the action name from the route data.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The action name, or "unknown" if not available.</returns>
    /// <example>
    /// <code>
    /// string action = httpContext.GetActionName();
    /// </code>
    /// </example>
    public static string? GetActionName(this HttpContext context)
    {
        var routeValues = GetRouteValue(context);
        return routeValues.TryGetValue("action", out var value) ? value?.ToString() : "unknown";
    }

    /// <summary>
    /// Gets the route value dictionary from the context.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A RouteValueDictionary containing the route values.</returns>
    private static RouteValueDictionary GetRouteValue(HttpContext context)
    {
        var routeData = context.GetRouteData();
        var routeValues = routeData.Values;

        return routeValues;
    }

    #endregion

    #region QueryString

    /// <summary>
    /// Gets a value from the query string.
    /// </summary>
    /// <param name="request">The HttpContext instance.</param>
    /// <param name="key">The query string key.</param>
    /// <returns>The value of the specified query string parameter, or null if not found.</returns>
    /// <example>
    /// <code>
    /// string id = httpContext.GetQueryStringValue("id");
    /// </code>
    /// </example>
    public static string? GetQueryStringValue(this HttpContext request, string key)
    {
        return request.Request.Query.TryGetValue(key, out var values)
            ? values.FirstOrDefault()
            : null;
    }

    /// <summary>
    /// Converts all query string parameters to a dictionary.
    /// </summary>
    /// <param name="request">The HttpContext instance.</param>
    /// <returns>A dictionary containing all query string parameters.</returns>
    /// <example>
    /// <code>
    /// var queryParams = httpContext.GetQueryStringToDictionary();
    /// foreach (var param in queryParams)
    /// {
    ///     Console.WriteLine($"{param.Key}: {param.Value}");
    /// }
    /// </code>
    /// </example>
    public static Dictionary<string, string> GetQueryStringToDictionary(this HttpContext request)
    {
        var query = new Dictionary<string, string>();
        foreach (var queryItem in request.Request.Query)
        {
            query[queryItem.Key] = queryItem.Value.ToString();
        }

        return query;
    }

    /// <summary>
    /// Converts all query string parameters to a JSON string.
    /// </summary>
    /// <param name="request">The HttpContext instance.</param>
    /// <returns>A JSON string containing all query string parameters.</returns>
    /// <example>
    /// <code>
    /// string queryParamsJson = httpContext.GetQueryStringToJson();
    /// </code>
    /// </example>
    public static string GetQueryStringToJson(this HttpContext request)
    {
        return JsonSerializer.Serialize(request.GetQueryStringToDictionary(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    #endregion

    #region RequestBody

    /// <summary>
    /// Gets the request body as a string.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="maxLength">Maximum length of the request body to read. Default is 1000.</param>
    /// <returns>The request body as a string, truncated if it exceeds the maximum length.</returns>
    /// <remarks>
    /// This method properly handles the request body stream by enabling buffering and resetting the position.
    /// </remarks>
    /// <example>
    /// <code>
    /// string body = httpContext.GetRequestBody();
    /// </code>
    /// </example>
    public static string GetRequestBody(this HttpContext context, int maxLength = 1000)
    {
        if (context.Request.Body.CanSeek)
        {
            context.Request.Body.Position = 0;
        }
        else
        {
            context.Request.EnableBuffering();
        }

        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var originalContent = reader.ReadToEndAsync().Result;

        if (originalContent.Length > maxLength)
        {
            originalContent = originalContent.Substring(0, maxLength) + "... (truncated)";
        }

        context.Request.Body.Position = 0;

        return originalContent;
    }

    /// <summary>
    /// Gets the request body as a JSON string.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The request body serialized as a JSON string.</returns>
    /// <example>
    /// <code>
    /// string bodyJson = httpContext.GetRequestBodyToJson();
    /// </code>
    /// </example>
    public static string GetRequestBodyToJson(this HttpContext context)
    {
        var body = context.GetRequestBody();
        return JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    #endregion

    #region Cookies

    /// <summary>
    /// Gets the value of a cookie from the request.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="cookieName">The name of the cookie.</param>
    /// <returns>The value of the cookie, or null if not found.</returns>
    /// <example>
    /// <code>
    /// string sessionId = httpContext.GetCookieValue("SessionId");
    /// </code>
    /// </example>
    public static string? GetCookieValue(this HttpContext context, string cookieName)
    {
        return context.Request.Cookies.TryGetValue(cookieName, out var value)
            ? value
            : null;
    }

    /// <summary>
    /// Sets a cookie in the response.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="cookieName">The name of the cookie.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <param name="options">Optional CookieOptions to configure the cookie.</param>
    /// <example>
    /// <code>
    /// httpContext.SetCookieValue("SessionId", "abc123", new CookieOptions 
    /// { 
    ///     Expires = DateTime.Now.AddDays(1),
    ///     HttpOnly = true,
    ///     Secure = true
    /// });
    /// </code>
    /// </example>
    public static void SetCookieValue(this HttpContext context, string cookieName, string value, CookieOptions? options = null)
    {
        options ??= new CookieOptions();
        context.Response.Cookies.Append(cookieName, value, options);
    }

    /// <summary>
    /// Removes a cookie from the response.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="cookieName">The name of the cookie to remove.</param>
    /// <param name="options">Optional CookieOptions for the removal.</param>
    /// <example>
    /// <code>
    /// httpContext.RemoveCookie("SessionId");
    /// </code>
    /// </example>
    public static void RemoveCookie(this HttpContext context, string cookieName, CookieOptions? options = null)
    {
        options ??= new CookieOptions();
        context.Response.Cookies.Delete(cookieName, options);
    }

    /// <summary>
    /// Converts all cookies to a dictionary.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A dictionary containing all cookies.</returns>
    /// <example>
    /// <code>
    /// var cookies = httpContext.GetAllCookiesToDictionary();
    /// foreach (var cookie in cookies)
    /// {
    ///     Console.WriteLine($"{cookie.Key}: {cookie.Value}");
    /// }
    /// </code>
    /// </example>
    public static Dictionary<string, string> GetAllCookiesToDictionary(this HttpContext context)
    {
        var cookies = new Dictionary<string, string>();
        foreach (var cookie in context.Request.Cookies)
        {
            cookies[cookie.Key] = cookie.Value;
        }
        return cookies;
    }

    /// <summary>
    /// Converts all cookies to a JSON string.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A JSON string containing all cookies.</returns>
    /// <example>
    /// <code>
    /// string cookiesJson = httpContext.GetAllCookiesToJson();
    /// </code>
    /// </example>
    public static string GetAllCookiesToJson(this HttpContext context)
    {
        return JsonSerializer.Serialize(context.GetAllCookiesToDictionary(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    #endregion

    #region Session

    /// <summary>
    /// Sets a serialized value in the session.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The session key.</param>
    /// <param name="value">The value to store.</param>
    /// <remarks>
    /// The value is serialized to JSON before being stored in the session.
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = new User { Id = 1, Name = "John" };
    /// httpContext.SetSessionValue("CurrentUser", user);
    /// </code>
    /// </example>
    public static void SetSessionValue<T>(this HttpContext context, string key, T value)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        context.Session.SetString(key, serializedValue);
    }

    /// <summary>
    /// Gets a serialized value from the session.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The session key.</param>
    /// <returns>The deserialized value, or default if not found.</returns>
    /// <remarks>
    /// The value is deserialized from JSON after being retrieved from the session.
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = httpContext.GetSessionValue >User>("CurrentUser");
    /// if (user != null)
    /// {
    ///     Console.WriteLine($"User: {user.Name}");
    /// }
    /// </code>
    /// </example>
    public static T? GetSessionValue<T>(this HttpContext context, string key)
    {
        var value = context.Session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// Removes a value from the session.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The session key to remove.</param>
    /// <example>
    /// <code>
    /// httpContext.RemoveSessionValue("CurrentUser");
    /// </code>
    /// </example>
    public static void RemoveSessionValue(this HttpContext context, string key)
    {
        context.Session.Remove(key);
    }

    /// <summary>
    /// Checks if a key exists in the session.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The session key to check.</param>
    /// <returns>True if the key exists in the session, otherwise false.</returns>
    /// <example>
    /// <code>
    /// if (httpContext.HasSessionValue("CurrentUser"))
    /// {
    ///     var user = httpContext.GetSessionValue >User>("CurrentUser");
    /// }
    /// </code>
    /// </example>
    public static bool HasSessionValue(this HttpContext context, string key)
    {
        return context.Session.Keys.Contains(key);
    }

    #endregion

    #region Form Data

    /// <summary>
    /// Gets a value from the form data asynchronously.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The form field key.</param>
    /// <returns>The value of the specified form field, or null if not found.</returns>
    /// <example>
    /// <code>
    /// string name = await httpContext.GetFormValueAsync("name");
    /// </code>
    /// </example>
    public static async Task<string?> GetFormValueAsync(this HttpContext context, string key)
    {
        var form = await context.Request.ReadFormAsync();
        return form.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;
    }

    /// <summary>
    /// Converts all form data to a dictionary asynchronously.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>A dictionary containing all form fields.</returns>
    /// <example>
    /// <code>
    /// var formData = await httpContext.GetFormDataToDictionaryAsync();
    /// foreach (var field in formData)
    /// {
    ///     Console.WriteLine($"{field.Key}: {field.Value}");
    /// }
    /// </code>
    /// </example>
    public static async Task<Dictionary<string, string>> GetFormDataToDictionaryAsync(this HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        var formData = new Dictionary<string, string>();

        foreach (var item in form)
        {
            formData[item.Key] = item.Value.ToString();
        }

        return formData;
    }

    /// <summary>
    /// Gets a file from the form data asynchronously.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The form file key.</param>
    /// <returns>The IFormFile, or null if not found.</returns>
    /// <example>
    /// <code>
    /// var file = await httpContext.GetFormFileAsync("profilePicture");
    /// if (file != null && file.Length > 0)
    /// {
    ///     // Process the file
    /// }
    /// </code>
    /// </example>
    public static async Task<IFormFile?> GetFormFileAsync(this HttpContext context, string key)
    {
        var form = await context.Request.ReadFormAsync();
        return form.Files.GetFile(key);
    }

    /// <summary>
    /// Gets multiple files for a single key from the form data asynchronously.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <param name="key">The form file key.</param>
    /// <returns>A list of IFormFile objects.</returns>
    /// <example>
    /// <code>
    /// var files = await httpContext.GetFormFilesAsync("attachments");
    /// foreach (var file in files)
    /// {
    ///     // Process each file
    /// }
    /// </code>
    /// </example>
    public static async Task<List<IFormFile>> GetFormFilesAsync(this HttpContext context, string key)
    {
        var form = await context.Request.ReadFormAsync();
        return form.Files.GetFiles(key).ToList();
    }

    #endregion
}