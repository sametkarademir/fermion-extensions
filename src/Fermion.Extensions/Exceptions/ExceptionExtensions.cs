using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Fermion.Extensions.Exceptions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Generates a fingerprint for the exception using SHA256 hashing algorithm.
    /// </summary>
    /// <param name="exception">The exception to generate a fingerprint for.</param>
    /// <returns>A base64 encoded string representing the fingerprint of the exception.</returns>
    /// <remarks>
    /// This method creates a unique fingerprint for the exception based on its type, message, and stack trace.
    /// The stack trace is truncated to a maximum length of 500 characters to ensure consistent fingerprint generation.
    /// </remarks>
    /// <example>
    /// <code>
    /// var exception = new Exception("An error occurred.");
    /// var fingerprint = exception.GenerateFingerprint();
    /// Console.WriteLine(fingerprint);
    /// </code>
    /// </example>
    public static string GenerateFingerprint(this Exception exception)
    {
        using var sha = SHA256.Create();

        var exceptionType = exception.GetType().FullName ?? "UnknownType";
        var message = exception.Message;
        var stackTrace = exception.StackTrace?.Substring(0, Math.Min(500, (exception.StackTrace?.Length ?? 0))) ?? string.Empty;

        var input = $"{exceptionType}|{message}|{stackTrace}";
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Converts the exception data to a dictionary.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <returns>A dictionary containing the exception data.</returns>
    /// <remarks>
    /// This method iterates through the exception's data collection and adds each key-value pair to a dictionary.
    /// The keys are converted to strings, and null values are skipped.
    /// </remarks>
    /// <example>
    /// <code>
    /// var exception = new Exception("An error occurred.");
    /// exception.Data["Key1"] = "Value1";
    /// exception.Data["Key2"] = null;
    /// var data = exception.ConvertExceptionDataToDictionary();
    /// Console.WriteLine(data);
    /// </code>
    /// </example>
    public static Dictionary<string, object> ConvertExceptionDataToDictionary(this Exception exception)
    {
        var data = new Dictionary<string, object>(exception.Data.Count);
        if (exception.Data.Count > 0)
        {
            foreach (var keyObject in exception.Data.Keys)
            {
                var key = keyObject.ToString();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var value = exception.Data[keyObject];
                if (value != null)
                {
                    data.Add(key, value);
                }
            }
        }

        return data;
    }

    /// <summary>
    /// Converts the exception data to a JSON string.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <returns>A JSON string representing the exception data.</returns>
    /// <remarks>
    /// This method iterates through the exception's data collection and serializes it to a JSON string.
    /// The keys are converted to strings, and null values are skipped.
    /// </remarks>
    /// <example>
    /// <code>
    /// var exception = new Exception("An error occurred.");
    /// exception.Data["Key1"] = "Value1";
    /// exception.Data["Key2"] = null;
    /// var json = exception.ConvertExceptionDataToJson();
    /// Console.WriteLine(json);
    /// </code>
    /// </example>
    public static string ConvertExceptionDataToJson(this Exception exception)
    {
        try
        {
            var data = ConvertExceptionDataToDictionary(exception);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(data, options);
        }
        catch (Exception e)
        {
            return $"{{\"SerializationError\": \"{e.Message}\"}}";
        }
    }

    /// <summary>
    /// Converts the inner exceptions to the exception to a list of dictionaries.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <returns>A list of dictionaries representing the inner exceptions.</returns>
    /// <remarks>
    /// This method iterates through the inner exceptions to the exception and adds each inner exception's type, message, and stack trace to a dictionary.
    /// The list contains all inner exceptions in the order they are nested.
    /// </remarks>
    /// <example>
    /// <code>
    /// var innerException = new Exception("Inner exception.");
    /// var outerException = new Exception("Outer exception.", innerException);
    /// var innerExceptionsList = outerException.ConvertInnerExceptionsToList();
    /// Console.WriteLine(innerExceptionsList);
    /// </code>
    /// </example>
    public static List<Dictionary<string, string>> ConvertInnerExceptionsToList(this Exception exception)
    {
        var innerExceptionsList = new List<Dictionary<string, string>>();
        var innerException = exception.InnerException;
        var depth = 0;

        while (innerException != null)
        {
            innerExceptionsList.Add(new Dictionary<string, string>
            {
                { "Type", innerException.GetType().FullName ?? string.Empty },
                { "Message", innerException.Message },
                { "StackTrace", innerException.StackTrace ?? string.Empty },
                { "Depth", depth.ToString() }
            });
            innerException = innerException.InnerException;
            depth++;
        }

        return innerExceptionsList;
    }

    /// <summary>
    /// Converts the inner exceptions to the exception to a JSON string.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <returns>A JSON string representing the inner exceptions.</returns>
    /// <remarks>
    /// This method iterates through the inner exceptions to the exception and serializes them to a JSON string.
    /// Each inner exception's type, message, and stack trace are included in the JSON representation.
    /// </remarks>
    /// <example>
    /// <code>
    /// var innerException = new Exception("Inner exception.");
    /// var outerException = new Exception("Outer exception.", innerException);
    /// var json = outerException.ConvertInnerExceptionsToJson();
    /// Console.WriteLine(json);
    /// </code>
    /// </example>
    public static string ConvertInnerExceptionsToJson(this Exception exception)
    {
        try
        {
            var innerExceptionsList = ConvertInnerExceptionsToList(exception);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(innerExceptionsList, options);
        }
        catch (Exception e)
        {
            return $"{{\"SerializationError\": \"{e.Message}\"}}";
        }
    }

    /// <summary>
    /// Gets the type of the exception as a string.
    /// </summary>
    /// <param name="exception">The exception to get the type of.</param>
    /// <returns>A string representing the type of the exception.</returns>
    /// <remarks>
    /// This method retrieves the full name of the exception type.
    /// If the type is null, it returns an empty string.
    /// </remarks>
    /// <example>
    /// <code>
    /// var exception = new Exception("An error occurred.");
    /// var type = exception.GetExceptionType();
    /// Console.WriteLine(type);
    /// </code>
    /// </example>
    public static string GetExceptionType(this Exception exception)
    {
        return exception.GetType().FullName ?? "UnknownType";
    }

    /// <summary>
    /// Gets detailed stack trace information for the exception.
    /// </summary>
    /// <param name="exception">The exception to get the stack trace info for.</param>
    /// <param name="includeSource">Whether to include source file and line number information.</param>
    /// <param name="includeTimestamp">Whether to include timestamp information for each frame.</param>
    /// <returns>A string containing detailed stack trace information.</returns>
    /// <remarks>
    /// This method creates a detailed representation of the stack trace with method names, parameter types, and source file information if available.
    /// If includeTimestamp is true, it also adds the current timestamp to each frame.
    /// </remarks>
    /// <example>
    /// <code>
    /// var exception = new Exception("An error occurred.");
    /// var stackTraceInfo = exception.GetStackTraceInfo(includeSource: true, includeTimestamp: true);
    /// Console.WriteLine(stackTraceInfo);
    /// </code>
    /// </example>
    public static string GetStackTraceInfo(
        this Exception exception,
        bool includeSource = false,
        bool includeTimestamp = false)
    {
        var stackTrace = new StackTrace(exception, includeSource);
        var sb = new StringBuilder();
        var timestamp = DateTime.Now;

        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            if (method != null)
            {
                var typeName = method.DeclaringType?.FullName ?? "<unknown>";
                var methodName = method.Name;
                var parameters = string.Join(", ", method.GetParameters()
                    .Select(p => $"{p.ParameterType.Name} {p.Name}"));

                sb.AppendLine($"  at {typeName}.{methodName}({parameters})");

                if (includeTimestamp)
                {
                    sb.Append($" [{timestamp:yyyy-MM-dd HH:mm:ss.fff}]");
                }

                sb.AppendLine();

                if (includeSource && frame.GetFileName() != null)
                {
                    var fileName = Path.GetFileName(frame.GetFileName());
                    var line = frame.GetFileLineNumber();
                    sb.AppendLine($"    in {fileName}:line {line}");
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Creates a detailed error report including exception information and application context.
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    /// <param name="includeLocationInfo">Whether to include detailed location information.</param>
    /// <param name="includeStackFrames">Whether to include application stack frames.</param>
    /// <param name="includeEnvironmentInfo">Whether to include environment information.</param>
    /// <param name="serviceNamespacePrefix">Optional namespace prefix to identify service classes.</param>
    /// <param name="correlationId">Optional correlation ID for tracking.</param>
    /// <returns>A dictionary containing all the error information.</returns>
    /// <remarks>
    /// This method collects all relevant information about the exception and creates a comprehensive error report.
    /// The report includes exception details, location information, stack frames, and environment information.
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Some code that might throw
    /// }
    /// catch (Exception ex)
    /// {
    ///     var errorReport = ex.CreateErrorReport(serviceNamespacePrefix: "MyCompany.Services");
    ///     _logger.LogError("Detailed error: {@ErrorReport}", errorReport);
    /// }
    /// </code>
    /// </example>
    public static Dictionary<string, object> CreateErrorReport(
        this Exception exception,
        bool includeLocationInfo = true,
        bool includeStackFrames = true,
        bool includeEnvironmentInfo = true,
        string? serviceNamespacePrefix = null,
        string? correlationId = null)
    {
        var report = new Dictionary<string, object>
        {
            { "Timestamp", DateTime.UtcNow },
            { "ExceptionType", exception.GetType().FullName ?? "<unknown>" },
            { "Message", exception.Message },
            { "Source", exception.Source ?? "<unknown>" },
            { "Fingerprint", exception.GenerateFingerprint() },
        };

        if (!string.IsNullOrEmpty(correlationId))
        {
            report["CorrelationId"] = correlationId;
        }

        var exceptionData = exception.ConvertExceptionDataToDictionary();
        if (exceptionData.Count > 0)
        {
            report["ExceptionData"] = exceptionData;
        }

        var innerExceptions = exception.ConvertInnerExceptionsToList();
        if (innerExceptions.Count > 0)
        {
            report["InnerExceptions"] = innerExceptions;
        }

        if (includeLocationInfo)
        {
            report["LocationInfo"] = exception.GetExceptionLocationInfo(
                skipSystemFrames: true,
                serviceNamespacePrefix: serviceNamespacePrefix);
        }

        if (includeStackFrames)
        {
            report["ApplicationStackFrames"] = exception.GetApplicationStackFrames(
                skipSystemFrames: true,
                maxFrames: 10);
        }

        if (includeEnvironmentInfo)
        {
            report["Environment"] = new Dictionary<string, string>
            {
                { "MachineName", Environment.MachineName },
                { "OSVersion", Environment.OSVersion.ToString() },
                { "ApplicationDomain", AppDomain.CurrentDomain.FriendlyName },
                { "ProcessId", Environment.ProcessId.ToString() },
                { "ProcessStartTime", Process.GetCurrentProcess().StartTime.ToString("o") },
                { "ProcessorCount", Environment.ProcessorCount.ToString() },
                { "RuntimeVersion", Environment.Version.ToString() }
            };
        }

        return report;
    }

    /// <summary>
    /// Gets all relevant application frames from the exception stack trace.
    /// </summary>
    /// <param name="exception">The exception to analyze.</param>
    /// <param name="skipSystemFrames">Whether to skip system framework frames.</param>
    /// <param name="maxFrames">Maximum number of frames to return.</param>
    /// <returns>A list of dictionaries with information about each relevant stack frame.</returns>
    /// <example>
    /// <code>
    /// var exception = new Exception("Operation failed");
    /// var frames = exception.GetApplicationStackFrames();
    /// foreach (var frame in frames) {
    ///     Console.WriteLine($"{frame["ClassName"]}.{frame["MethodName"]}");
    /// }
    /// </code>
    /// </example>
    public static List<Dictionary<string, string>> GetApplicationStackFrames(
        this Exception exception,
        bool skipSystemFrames = true,
        int maxFrames = 10)
    {
        var result = new List<Dictionary<string, string>>();
        var stackTrace = new StackTrace(exception, true);
        var frames = stackTrace.GetFrames();

        var count = 0;
        foreach (var frame in frames)
        {
            if (count >= maxFrames)
            {
                break;
            }

            var method = frame.GetMethod();
            if (method == null || method.DeclaringType == null)
            {
                continue;
            }

            var ns = method.DeclaringType.Namespace;
            if (skipSystemFrames && (
                    ns?.StartsWith("System.") == true ||
                    ns?.StartsWith("Microsoft.") == true ||
                    ns?.StartsWith("Newtonsoft.") == true))
            {
                continue;
            }

            var className = method.DeclaringType.Name;
            var methodName = method.Name;
            var fullTypeName = method.DeclaringType.FullName ?? "<unknown>";

            if (className.Contains("<") && className.Contains(">d__"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(className, @"\<([^>]+)\>");
                if (match.Success)
                {
                    methodName = match.Groups[1].Value;
                    var plusIndex = fullTypeName.IndexOf('+');
                    if (plusIndex > 0)
                    {
                        string actualTypeName = fullTypeName.Substring(0, plusIndex);
                        className = actualTypeName.Substring(actualTypeName.LastIndexOf('.') + 1);
                        fullTypeName = actualTypeName;
                    }
                }
            }

            var frameInfo = new Dictionary<string, string>
            {
                { "Namespace", ns ?? "<unknown>" },
                { "ClassName", className },
                { "MethodName", methodName },
                { "FullTypeName", fullTypeName }
            };

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                frameInfo["Parameters"] = string.Join(", ",
                    parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            }

            if (frame.GetFileName() != null)
            {
                frameInfo["SourceFile"] = Path.GetFileName(frame.GetFileName() ?? "<unknown>");
                frameInfo["LineNumber"] = frame.GetFileLineNumber().ToString();
            }

            frameInfo["ApplicationLayer"] = DetermineApplicationLayer(ns ?? string.Empty, method.DeclaringType.Name);

            result.Add(frameInfo);
            count++;
        }

        return result;
    }


    /// <summary>
    /// Gets specific information about where the exception occurred in the application.
    /// </summary>
    /// <param name="exception">The exception to analyze.</param>
    /// <param name="skipSystemFrames">Whether to skip system framework frames.</param>
    /// <param name="serviceNamespacePrefix">Optional namespace prefix to identify service classes.</param>
    /// <returns>A dictionary containing specific information about where the exception occurred.</returns>
    /// <example>
    /// <code>
    /// var exception = new Exception("Database connection failed");
    /// var locationInfo = exception.GetExceptionLocationInfo("MyCompany.Services");
    /// // Returns dictionary with keys like "ServiceName", "ClassName", "MethodName", "Namespace", etc.
    /// Console.WriteLine($"Error in service: {locationInfo["ServiceName"]}");
    /// Console.WriteLine($"Error in method: {locationInfo["MethodName"]}");
    /// </code>
    /// </example>
    public static Dictionary<string, string> GetExceptionLocationInfo(
        this Exception exception,
        bool skipSystemFrames = true,
        string? serviceNamespacePrefix = null)
    {
        var result = new Dictionary<string, string>
        {
            { "ExceptionType", exception.GetType().Name }
        };

        var stackTrace = new StackTrace(exception, true);
        var frames = stackTrace.GetFrames();

        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            if (method == null || method.DeclaringType == null)
            {
                continue;
            }

            var ns = method.DeclaringType.Namespace;
            if (skipSystemFrames && (
                    ns?.StartsWith("System.") == true ||
                    ns?.StartsWith("Microsoft.") == true ||
                    ns?.StartsWith("Newtonsoft.") == true))
            {
                continue;
            }

            var className = method.DeclaringType.Name;
            var methodName = method.Name;
            var fullTypeName = method.DeclaringType.FullName ?? "<unknown>";

            if (className.Contains("<") && className.Contains(">d__"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(className, @"\<([^>]+)\>");
                if (match.Success)
                {
                    methodName = match.Groups[1].Value;
                    var plusIndex = fullTypeName.IndexOf('+');
                    if (plusIndex > 0)
                    {
                        string actualTypeName = fullTypeName.Substring(0, plusIndex);
                        className = actualTypeName.Substring(actualTypeName.LastIndexOf('.') + 1);
                        fullTypeName = actualTypeName;
                    }
                }
            }

            result["Namespace"] = ns ?? "<unknown>";
            result["ClassName"] = className;
            result["FullTypeName"] = fullTypeName;
            result["MethodName"] = methodName;

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                result["Parameters"] = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            }

            if (frame.GetFileName() != null)
            {
                result["SourceFile"] = Path.GetFileName(frame.GetFileName() ?? "<unknown>");
                result["LineNumber"] = frame.GetFileLineNumber().ToString();
            }

            var serviceName = DetermineServiceName(
                className,
                ns ?? "<unknown>",
                serviceNamespacePrefix);

            if (!string.IsNullOrEmpty(serviceName))
            {
                result["ServiceName"] = serviceName;
            }

            result["ApplicationLayer"] = DetermineApplicationLayer(ns ?? "<unknown>", className);

            break;
        }

        return result;
    }

    private static string DetermineServiceName(
        string className,
        string ns,
        string? serviceNamespacePrefix)
    {
        if (className.EndsWith("Service") ||
            className.EndsWith("Manager") ||
            className.EndsWith("Provider") ||
            className.EndsWith("Repository") ||
            className.EndsWith("Handler"))
        {
            return className;
        }

        if (!string.IsNullOrEmpty(serviceNamespacePrefix) && ns.StartsWith(serviceNamespacePrefix, StringComparison.OrdinalIgnoreCase))
        {
            var nsWithoutPrefix = ns.Substring(serviceNamespacePrefix.Length).TrimStart('.');
            var parts = nsWithoutPrefix.Split('.');
            if (parts.Length > 0)
            {
                return parts[0] + "Service";
            }
        }

        return string.Empty;
    }

    private static string DetermineApplicationLayer(string ns, string className)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return "<unknown>";
        }

        if (ns.Contains(".Controllers.") || ns.Contains(".Api.") || className.EndsWith("Controller"))
        {
            return "API";
        }

        if (ns.Contains(".Services.") || className.EndsWith("Service"))
        {
            return "Service";
        }

        if (ns.Contains(".Repositories.") || className.EndsWith("Repository") || className.EndsWith("Repo"))
        {
            return "Repository";
        }

        if (ns.Contains(".Domain.") || ns.Contains(".Models.") || ns.Contains(".Entities."))
        {
            return "Domain";
        }

        if (ns.Contains(".Data.") || ns.Contains(".Infrastructure.") || ns.Contains(".Persistence."))
        {
            return "Infrastructure";
        }

        if (ns.Contains(".UI.") || ns.Contains(".Views.") || ns.Contains(".Pages."))
        {
            return "UI";
        }

        return "Application";
    }
}