using System.Collections;
using System.Linq.Expressions;

namespace Fermion.Extensions.Linq;

public class LinqExtensionsTests
{
    private readonly List<TestPerson> _testData =
    [
        new TestPerson { Id = 1, Name = "Alice", Age = 25, City = "New York" },
        new TestPerson { Id = 2, Name = "Bob", Age = 30, City = "Los Angeles" },
        new TestPerson { Id = 3, Name = "Charlie", Age = 35, City = "Chicago" },
        new TestPerson { Id = 4, Name = "David", Age = 40, City = "Houston" },
        new TestPerson { Id = 5, Name = "Eve", Age = 45, City = "Phoenix" }
    ];

    #region WhereIf Tests - IQueryable

    [Fact]
    public void WhereIf_IQueryable_WhenConditionTrue_ShouldApplyFilter()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        Expression<Func<TestPerson, bool>> predicate = p => p.Age > 30;

        // Act
        var result = query.WhereIf(condition, predicate).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.All(result, p => Assert.True(p.Age > 30));
    }

    [Fact]
    public void WhereIf_IQueryable_WhenConditionFalse_ShouldNotApplyFilter()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = false;
        Expression<Func<TestPerson, bool>> predicate = p => p.Age > 30;

        // Act
        var result = query.WhereIf(condition, predicate).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(_testData, result);
    }

    [Fact]
    public void WhereIf_IQueryable_WithMultipleConditions_ShouldChainFilters()
    {
        // Arrange
        var query = _testData.AsQueryable();

        // Act
        var result = query
            .WhereIf(true, p => p.Age > 30)
            .WhereIf(true, p => p.Name != null && p.Name.StartsWith("C"))
            .ToList();

        // Assert
        Assert.Single((IEnumerable)result);
        Assert.Equal("Charlie", result[0].Name);
    }

    #endregion

    #region WhereIf Tests - IEnumerable

    [Fact]
    public void WhereIf_IEnumerable_WhenConditionTrue_ShouldApplyFilter()
    {
        // Arrange
        const bool condition = true;
        Func<TestPerson, bool> predicate = p => p.Age > 30;

        // Act
        var result = _testData.WhereIf(condition, predicate).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.All(result, p => Assert.True(p.Age > 30));
    }

    [Fact]
    public void WhereIf_IEnumerable_WhenConditionFalse_ShouldNotApplyFilter()
    {
        // Arrange
        const bool condition = false;
        Func<TestPerson, bool> predicate = p => p.Age > 30;

        // Act
        var result = _testData.WhereIf(condition, predicate).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(_testData, result);
    }

    [Fact]
    public void WhereIf_IEnumerable_WithMultipleConditions_ShouldChainFilters()
    {
        // Arrange

        // Act
        var result = _testData
            .WhereIf(true, p => p.Age > 30)
            .WhereIf(true, p => p.Name != null && p.Name.StartsWith("C"))
            .ToList();

        // Assert
        Assert.Single((IEnumerable)result);
        Assert.Equal("Charlie", result[0].Name);
    }

    #endregion

    #region OrderByIf Tests

    [Fact]
    public void OrderByIf_WhenConditionTrue_AndAscendingTrue_ShouldApplyAscendingSort()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        Expression<Func<TestPerson, int>> keySelector = p => p.Age;

        // Act
        var result = query.OrderByIf(condition, keySelector).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(25, result[0].Age);
        Assert.Equal(30, result[1].Age);
        Assert.Equal(35, result[2].Age);
        Assert.Equal(40, result[3].Age);
        Assert.Equal(45, result[4].Age);
    }

    [Fact]
    public void OrderByIf_WhenConditionTrue_AndAscendingFalse_ShouldApplyDescendingSort()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        Expression<Func<TestPerson, int>> keySelector = p => p.Age;

        // Act
        var result = query.OrderByIf(condition, keySelector, false).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(45, result[0].Age);
        Assert.Equal(40, result[1].Age);
        Assert.Equal(35, result[2].Age);
        Assert.Equal(30, result[3].Age);
        Assert.Equal(25, result[4].Age);
    }

    [Fact]
    public void OrderByIf_WhenConditionFalse_ShouldNotApplySort()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = false;
        Expression<Func<TestPerson, int>> keySelector = p => p.Age;

        // Act
        var result = query.OrderByIf(condition, keySelector).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(_testData, result); // Order should remain the same
    }

    #endregion

    #region ThenByIf Tests

    [Fact]
    public void ThenByIf_WhenConditionTrue_AndAscendingTrue_ShouldApplyAscendingSecondarySort()
    {
        // Arrange
        const bool condition = true;

        // Create duplicate ages to test secondary sort
        var testDataWithDuplicateAges = new List<TestPerson>
        {
            new TestPerson { Id = 1, Name = "Zack", Age = 30, City = "New York" },
            new TestPerson { Id = 2, Name = "Bob", Age = 30, City = "Los Angeles" },
            new TestPerson { Id = 3, Name = "Alice", Age = 30, City = "Chicago" }
        };

        var queryWithDuplicates = testDataWithDuplicateAges.AsQueryable();

        // Act
        var result = queryWithDuplicates
            .OrderBy(p => p.Age)
            .ThenByIf(condition, p => p.Name)
            .ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal("Zack", result[2].Name);
    }

    [Fact]
    public void ThenByIf_WhenConditionTrue_AndAscendingFalse_ShouldApplyDescendingSecondarySort()
    {
        // Arrange
        const bool condition = true;

        // Create duplicate ages to test secondary sort
        var testDataWithDuplicateAges = new List<TestPerson>
        {
            new TestPerson { Id = 1, Name = "Zack", Age = 30, City = "New York" },
            new TestPerson { Id = 2, Name = "Bob", Age = 30, City = "Los Angeles" },
            new TestPerson { Id = 3, Name = "Alice", Age = 30, City = "Chicago" }
        };

        var queryWithDuplicates = testDataWithDuplicateAges.AsQueryable();

        // Act
        var result = queryWithDuplicates
            .OrderBy(p => p.Age)
            .ThenByIf(condition, p => p.Name, false)
            .ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Zack", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal("Alice", result[2].Name);
    }

    [Fact]
    public void ThenByIf_WhenConditionFalse_ShouldNotApplySecondarySort()
    {
        // Arrange
        // Create duplicate ages to test secondary sort
        var testDataWithDuplicateAges = new List<TestPerson>
        {
            new TestPerson { Id = 1, Name = "Zack", Age = 30, City = "New York" },
            new TestPerson { Id = 2, Name = "Bob", Age = 30, City = "Los Angeles" },
            new TestPerson { Id = 3, Name = "Alice", Age = 30, City = "Chicago" }
        };

        var queryWithDuplicates = testDataWithDuplicateAges.AsQueryable();
        var condition = false;

        // Act
        var result = queryWithDuplicates
            .OrderBy(p => p.Age)
            .ThenByIf(condition, p => p.Name)
            .ToList();

        // Assert
        Assert.Equal(3, result.Count);
        // Without secondary sort, order is indeterminate for duplicate ages,
        // so we just verify all items are present
        Assert.Contains(result, p => p.Name == "Zack");
        Assert.Contains(result, p => p.Name == "Bob");
        Assert.Contains(result, p => p.Name == "Alice");
    }

    #endregion

    #region SkipIf Tests

    [Fact]
    public void SkipIf_WhenConditionTrue_ShouldSkipSpecifiedCount()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        const int count = 2;

        // Act
        var result = query.SkipIf(condition, count).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(3, result[0].Id);
        Assert.Equal(4, result[1].Id);
        Assert.Equal(5, result[2].Id);
    }

    [Fact]
    public void SkipIf_WhenConditionFalse_ShouldNotSkipAny()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = false;
        const int count = 2;

        // Act
        var result = query.SkipIf(condition, count).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(_testData, result);
    }

    #endregion

    #region TakeIf Tests

    [Fact]
    public void TakeIf_WhenConditionTrue_ShouldTakeSpecifiedCount()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        const int count = 3;

        // Act
        var result = query.TakeIf(condition, count).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
        Assert.Equal(3, result[2].Id);
    }

    [Fact]
    public void TakeIf_WhenConditionFalse_ShouldTakeAll()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = false;
        const int count = 3;

        // Act
        var result = query.TakeIf(condition, count).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(_testData, result);
    }

    #endregion

    #region SelectIf Tests

    [Fact]
    public void SelectIf_WhenConditionTrue_ShouldUseMainSelector()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        Expression<Func<TestPerson, string>> mainSelector = p => p.Name ?? string.Empty;
        Expression<Func<TestPerson, string>> alternativeSelector = p => p.City ?? string.Empty;

        // Act
        var result = query.SelectIf(condition, mainSelector, alternativeSelector).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("Alice", result[0]);
        Assert.Equal("Bob", result[1]);
        Assert.Equal("Charlie", result[2]);
        Assert.Equal("David", result[3]);
        Assert.Equal("Eve", result[4]);
    }

    [Fact]
    public void SelectIf_WhenConditionFalse_ShouldUseAlternativeSelector()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = false;
        Expression<Func<TestPerson, string>> mainSelector = p => p.Name ?? string.Empty;
        Expression<Func<TestPerson, string>> alternativeSelector = p => p.City ?? string.Empty;

        // Act
        var result = query.SelectIf(condition, mainSelector, alternativeSelector).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("New York", result[0]);
        Assert.Equal("Los Angeles", result[1]);
        Assert.Equal("Chicago", result[2]);
        Assert.Equal("Houston", result[3]);
        Assert.Equal("Phoenix", result[4]);
    }

    [Fact]
    public void SelectIf_WithDifferentReturnTypes_ShouldWorkCorrectly()
    {
        // Arrange
        var query = _testData.AsQueryable();
        const bool condition = true;
        Expression<Func<TestPerson, object>> mainSelector = p => p.Name ?? string.Empty;
        Expression<Func<TestPerson, object>> alternativeSelector = p => p.Age;

        // Act
        var result = query.SelectIf(condition, mainSelector, alternativeSelector).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.IsType<string>(result[0]);
        Assert.Equal("Alice", result[0]);
    }

    #endregion

    #region Combination Tests

    [Fact]
    public void CombinationTest_ShouldChainMultipleMethods()
    {
        // Arrange
        var query = _testData.AsQueryable();

        // Act
        var result = query
            .WhereIf(true, p => p.Age >= 30)
            .OrderByIf(true, p => p.Name, false)
            .SkipIf(true, 1)
            .TakeIf(true, 2)
            .SelectIf<TestPerson, TestPerson>(true, p => new TestPerson { Name = p.Name, Age = p.Age }, p => new TestPerson { Name = p.Name, City = "Unknown" })
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);

        // Should be sorted by name descending after Charlie
        Assert.Equal("David", result[0].Name);
        Assert.Equal("Charlie", result[1].Name);

        // Ages should be 35 and 30
        Assert.Equal(40, result[0].Age);
        Assert.Equal(35, result[1].Age);
    }

    #endregion

    // Test data class
    private class TestPerson
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public int Age { get; init; }
        public string? City { get; init; }
    }
}