namespace Fermion.Extensions.Objects;

public class ObjectExtensionsTests
{
    [Fact]
    public void CastAs_SuccessfulCast_ReturnsCorrectType()
    {
        // Arrange
        object derived = new DerivedClass();
        object baseObj = new BaseClass();

        // Act
        var castedDerived = derived.CastAs<DerivedClass>();
        var castedToInterface = baseObj.CastAs<ITestInterface>();

        // Assert
        Assert.IsType<DerivedClass>(castedDerived);
        Assert.IsAssignableFrom<ITestInterface>(castedToInterface);
    }

    [Fact]
    public void CastAs_InvalidCast_ThrowsInvalidCastException()
    {
        // Arrange
        object baseObj = new BaseClass();

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => baseObj.CastAs<string>());
    }

    [Fact]
    public void ConvertTo_IntConversion_ReturnsCorrectValue()
    {
        // Arrange
        object intString = "42";
        object doubleValue = 42.5;

        // Act
        var intResult = intString.ConvertTo<int>();
        var doubleToInt = doubleValue.ConvertTo<int>();

        // Assert
        Assert.Equal(42, intResult);
        Assert.Equal(42, doubleToInt);
    }

    [Fact]
    public void ConvertTo_GuidConversion_ReturnsCorrectValue()
    {
        // Arrange
        const string guidString = "12345678-1234-1234-1234-123456789012";
        var expectedGuid = new Guid(guidString);

        // Act
        var result = guidString.ConvertTo<Guid>();

        // Assert
        Assert.Equal(expectedGuid, result);
    }

    [Fact]
    public void ConvertTo_InvalidConversion_ThrowsException()
    {
        // Arrange
        object invalidValue = "not a number";

        // Act & Assert
        Assert.Throws<FormatException>(() => invalidValue.ConvertTo<int>());
    }

    [Fact]
    public void ExistsInCollection_WithParams_ItemExists_ReturnsTrue()
    {
        // Arrange
        const int item = 42;

        // Act
        var result = item.ExistsInCollection(10, 20, 42, 50);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ExistsInCollection_WithParams_ItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string item = "İstanbul";

        // Act
        var result = item.ExistsInCollection("Ankara", "İzmir", "Bursa");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExistsInCollection_WithEnumerable_ItemExists_ReturnsTrue()
    {
        // Arrange
        const int item = 42;
        var collection = new List<int> { 10, 20, 42, 50 };

        // Act
        var result = item.ExistsInCollection(collection);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ExistsInCollection_WithEnumerable_ItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string item = "İstanbul";
        IEnumerable<string> collection = ["Ankara", "İzmir", "Bursa"];

        // Act
        var result = item.ExistsInCollection(collection);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoIf_WithFunc_ConditionTrue_ExecutesFunc()
    {
        // Arrange
        const string text = "hello world";
        const bool condition = true;

        // Act
        var result = text.DoIf(condition, t => t.ToUpper());

        // Assert
        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void DoIf_WithFunc_ConditionFalse_DoesNotExecuteFunc()
    {
        // Arrange
        const int number = 10;
        const bool condition = false;

        // Act
        var result = number.DoIf(condition, n => n * 2);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public void DoIf_WithAction_ConditionTrue_ExecutesAction()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };
        const bool condition = true;

        // Act
        var result = list.DoIf(condition, l => l.Add(4));

        // Assert
        Assert.Equal(4, list.Count);
        Assert.Contains(4, list);
        Assert.Same(list, result); // Verify the original object is returned
    }

    [Fact]
    public void DoIf_WithAction_ConditionFalse_DoesNotExecuteAction()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };
        const bool condition = false;

        // Act
        var result = list.DoIf(condition, l => l.Clear());

        // Assert
        Assert.Equal(3, list.Count);
        Assert.Same(list, result); // Verify the original object is returned
    }

    [Fact]
    public void DoIf_ChainedCalls_ExecutesCorrectly()
    {
        // Arrange
        const int number = 10;
        const bool shouldMultiply = true;
        const bool shouldAdd = false;

        // Act
        var result = number
            .DoIf(shouldMultiply, n => n * 2)
            .DoIf(shouldAdd, n => n + 5);

        // Assert
        Assert.Equal(20, result);
    }

    // Helper classes for CastAs tests
    private interface ITestInterface
    {
    }

    private class BaseClass : ITestInterface
    {
    }

    private class DerivedClass : BaseClass
    {
    }
}