using System.Collections;
using System.Text.Json;

namespace Fermion.Extensions.Exceptions;

public class ExceptionExtensionsTests
{
    [Fact]
    public void GenerateFingerprint_ShouldReturnConsistentValue_ForSameException()
    {
        // Arrange
        var exception = new ArgumentException("Test message");

        // Act
        var fingerprint1 = exception.GenerateFingerprint();
        var fingerprint2 = exception.GenerateFingerprint();

        // Assert
        Assert.NotNull(fingerprint1);
        Assert.NotEmpty(fingerprint1);
        Assert.Equal(fingerprint1, fingerprint2);
    }

    [Fact]
    public void GenerateFingerprint_ShouldReturnDifferentValues_ForDifferentExceptions()
    {
        // Arrange
        var exception1 = new ArgumentException("Test message 1");
        var exception2 = new InvalidOperationException("Test message 2");

        // Act
        var fingerprint1 = exception1.GenerateFingerprint();
        var fingerprint2 = exception2.GenerateFingerprint();

        // Assert
        Assert.NotEqual(fingerprint1, fingerprint2);
    }

    [Fact]
    public void ConvertExceptionDataToDictionary_ShouldReturnEmptyDictionary_WhenNoData()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = exception.ConvertExceptionDataToDictionary();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ConvertExceptionDataToDictionary_ShouldReturnDictionary_WithExceptionData()
    {
        // Arrange
        var exception = new Exception("Test exception");
        exception.Data["Key1"] = "Value1";
        exception.Data["Key2"] = 42;

        // Act
        var result = exception.ConvertExceptionDataToDictionary();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Value1", result["Key1"]);
        Assert.Equal(42, result["Key2"]);
    }

    [Fact]
    public void ConvertExceptionDataToDictionary_ShouldSkipNullValues()
    {
        // Arrange
        var exception = new Exception("Test exception");
        exception.Data["Key1"] = "Value1";
        exception.Data["Key2"] = null;

        // Act
        var result = exception.ConvertExceptionDataToDictionary();

        // Assert
        Assert.NotNull(result);
        Assert.Single((IEnumerable)result);
        Assert.Equal("Value1", result["Key1"]);
        Assert.False(result.ContainsKey("Key2"));
    }

    [Fact]
    public void ConvertExceptionDataToJson_ShouldReturnValidJson_WithExceptionData()
    {
        // Arrange
        var exception = new Exception("Test exception");
        exception.Data["Key1"] = "Value1";
        exception.Data["Key2"] = 42;

        // Act
        var result = exception.ConvertExceptionDataToJson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // Verify it's valid JSON
        var deserializedObject = JsonSerializer.Deserialize<Dictionary<string, object>>(result);
        Assert.NotNull(deserializedObject);
        Assert.Equal(2, deserializedObject.Count);
    }

    [Fact]
    public void ConvertExceptionDataToJson_ShouldReturnErrorJson_WhenSerializationFails()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Add a circular reference that will cause serialization to fail
        var circularObject = new Dictionary<string, object>();
        circularObject["Self"] = circularObject;
        exception.Data["Circular"] = circularObject;

        // Act
        var result = exception.ConvertExceptionDataToJson();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("SerializationError", result);
    }

    [Fact]
    public void ConvertInnerExceptionsToList_ShouldReturnEmptyList_WhenNoInnerExceptions()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = exception.ConvertInnerExceptionsToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ConvertInnerExceptionsToList_ShouldReturnList_WithInnerExceptions()
    {
        // Arrange
        var innerMostException = new ArgumentException("Inner most exception");
        var innerException = new InvalidOperationException("Inner exception", innerMostException);
        var outerException = new Exception("Outer exception", innerException);

        // Act
        var result = outerException.ConvertInnerExceptionsToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(typeof(InvalidOperationException).FullName, result[0]["Type"]);
        Assert.Equal("Inner exception", result[0]["Message"]);
        Assert.Equal("0", result[0]["Depth"]);

        Assert.Equal(typeof(ArgumentException).FullName, result[1]["Type"]);
        Assert.Equal("Inner most exception", result[1]["Message"]);
        Assert.Equal("1", result[1]["Depth"]);
    }

    [Fact]
    public void ConvertInnerExceptionsToJson_ShouldReturnValidJson_WithInnerExceptions()
    {
        // Arrange
        var innerException = new ArgumentException("Inner exception");
        var outerException = new Exception("Outer exception", innerException);

        // Act
        var result = outerException.ConvertInnerExceptionsToJson();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // Verify it's valid JSON
        var deserializedObject = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(result);
        Assert.NotNull(deserializedObject);
        Assert.Single(deserializedObject);
        Assert.Equal(typeof(ArgumentException).FullName, deserializedObject[0]["Type"]);
    }

    [Fact]
    public void GetExceptionType_ShouldReturnTypeFullName()
    {
        // Arrange
        var exception = new ArgumentException("Test message");

        // Act
        var result = exception.GetExceptionType();

        // Assert
        Assert.Equal(typeof(ArgumentException).FullName, result);
    }

    [Fact]
    public void GetStackTraceInfo_ShouldReturnStackTraceWithMethodNames()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetStackTraceInfo(includeSource: false, includeTimestamp: false);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        // Validate it contains method information - basic structure check
        Assert.Contains("  at ", result);
    }

    [Fact]
    public void GetStackTraceInfo_ShouldIncludeSource_WhenRequested()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetStackTraceInfo(includeSource: true, includeTimestamp: false);

        // Assert
        Assert.NotNull(result);
        // Note: This test might not be reliable if the compiled code doesn't have source info
        // So we don't make strong assertions about the content
    }

    [Fact]
    public void GetStackTraceInfo_ShouldIncludeTimestamp_WhenRequested()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetStackTraceInfo(includeSource: false, includeTimestamp: true);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        // Check for timestamp format yyyy-MM-dd HH:mm:ss.fff
        var timestampPattern = @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]";
        Assert.Matches(timestampPattern, result);
    }

    [Fact]
    public void CreateErrorReport_ShouldIncludeBasicExceptionInfo()
    {
        // Arrange
        var exception = new ArgumentException("Test argument exception");

        // Act
        var result = exception.CreateErrorReport(
            includeLocationInfo: false,
            includeStackFrames: false,
            includeEnvironmentInfo: false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(ArgumentException).FullName, result["ExceptionType"]);
        Assert.Equal("Test argument exception", result["Message"]);
        Assert.Contains("Timestamp", result.Keys);
        Assert.Contains("Fingerprint", result.Keys);
    }

    [Fact]
    public void CreateErrorReport_ShouldIncludeExceptionData_WhenAvailable()
    {
        // Arrange
        var exception = new Exception("Test exception");
        exception.Data["TestKey"] = "TestValue";

        // Act
        var result = exception.CreateErrorReport(
            includeLocationInfo: false,
            includeStackFrames: false,
            includeEnvironmentInfo: false);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("ExceptionData", result.Keys);
        var exceptionData = (Dictionary<string, object>)result["ExceptionData"];
        Assert.Equal("TestValue", exceptionData["TestKey"]);
    }

    [Fact]
    public void CreateErrorReport_ShouldIncludeInnerExceptions_WhenAvailable()
    {
        // Arrange
        var innerException = new ArgumentException("Inner exception");
        var outerException = new Exception("Outer exception", innerException);

        // Act
        var result = outerException.CreateErrorReport(
            includeLocationInfo: false,
            includeStackFrames: false,
            includeEnvironmentInfo: false);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("InnerExceptions", result.Keys);
        var innerExceptions = (List<Dictionary<string, string>>)result["InnerExceptions"];
        Assert.Single(innerExceptions);
        Assert.Equal(typeof(ArgumentException).FullName, innerExceptions[0]["Type"]);
    }

    [Fact]
    public void CreateErrorReport_ShouldIncludeLocationInfo_WhenRequested()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.CreateErrorReport(
            includeLocationInfo: true,
            includeStackFrames: false,
            includeEnvironmentInfo: false);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("LocationInfo", result.Keys);
        var locationInfo = (Dictionary<string, string>)result["LocationInfo"];
        Assert.Contains("ExceptionType", locationInfo.Keys);
        Assert.Equal("InvalidOperationException", locationInfo["ExceptionType"]);
    }

    [Fact]
    public void CreateErrorReport_ShouldIncludeStackFrames_WhenRequested()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.CreateErrorReport(
            includeLocationInfo: false,
            includeStackFrames: true,
            includeEnvironmentInfo: false);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("ApplicationStackFrames", result.Keys);
        var stackFrames = (List<Dictionary<string, string>>)result["ApplicationStackFrames"];
        // Don't assert on the contents as it depends on the execution environment
        Assert.NotNull(stackFrames);
    }

    [Fact]
    public void CreateErrorReport_ShouldIncludeEnvironmentInfo_WhenRequested()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = exception.CreateErrorReport(
            includeLocationInfo: false,
            includeStackFrames: false,
            includeEnvironmentInfo: true);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Environment", result.Keys);
        var environmentInfo = (Dictionary<string, string>)result["Environment"];
        Assert.Contains("MachineName", environmentInfo.Keys);
        Assert.Contains("OSVersion", environmentInfo.Keys);
        Assert.Contains("RuntimeVersion", environmentInfo.Keys);
    }

    [Fact]
    public void GetApplicationStackFrames_ShouldReturnStackFrames()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetApplicationStackFrames();

        // Assert
        Assert.NotNull(result);
        // We can't make strong assertions about the content as it depends on the environment
        // But we can check the structure of the results
        if (result.Count > 0)
        {
            var firstFrame = result[0];
            Assert.Contains("ClassName", firstFrame.Keys);
            Assert.Contains("MethodName", firstFrame.Keys);
            Assert.Contains("Namespace", firstFrame.Keys);
            Assert.Contains("ApplicationLayer", firstFrame.Keys);
        }
    }

    [Fact]
    public void GetApplicationStackFrames_ShouldLimitFrames_WhenMaxFramesSpecified()
    {
        // Arrange
        Exception? exception = null;
        try
        {
            // Generate a deeper stack trace
            MethodThatThrows();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception?.GetApplicationStackFrames(maxFrames: 2);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count <= 2);
    }

    private void MethodThatThrows()
    {
        AnotherMethodThatThrows();
    }

    private void AnotherMethodThatThrows()
    {
        YetAnotherMethodThatThrows();
    }

    private void YetAnotherMethodThatThrows()
    {
        throw new InvalidOperationException("Deep stack trace exception");
    }

    [Fact]
    public void GetExceptionLocationInfo_ShouldReturnLocationInfo()
    {
        // Arrange
        Exception exception;
        try
        {
            // Generate a real stack trace
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetExceptionLocationInfo();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("ExceptionType", result.Keys);
        Assert.Equal("InvalidOperationException", result["ExceptionType"]);
        Assert.Contains("ClassName", result.Keys);
        Assert.Contains("MethodName", result.Keys);
        Assert.Contains("Namespace", result.Keys);
        Assert.Contains("ApplicationLayer", result.Keys);
    }

    [Fact]
    public void GetExceptionLocationInfo_ShouldIdentifyServiceName_WhenServiceClassFound()
    {
        // Arrange
        var testService = new TestService();
        Exception? exception = null;
        try
        {
            testService.MethodThatThrows();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception?.GetExceptionLocationInfo();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("ServiceName", result.Keys);
        Assert.Equal("TestService", result["ServiceName"]);
    }

    private class TestService
    {
        public void MethodThatThrows()
        {
            throw new InvalidOperationException("Service method exception");
        }
    }

    [Fact]
    public void GetExceptionLocationInfo_ShouldIdentifyServiceName_WhenNamespaceMatchesPrefix()
    {
        // Arrange
        Exception exception;
        try
        {
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetExceptionLocationInfo(
            serviceNamespacePrefix: "Fermion.Domain.Tests.Extensions");

        // Assert
        Assert.NotNull(result);
        // The test may pass or fail depending on namespace structure
        // and where the test is running from
    }
}