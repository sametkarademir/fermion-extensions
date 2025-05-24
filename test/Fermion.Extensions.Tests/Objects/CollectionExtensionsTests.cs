namespace Fermion.Extensions.Objects;

public class CollectionExtensionsTests
{
    #region IsNullOrEmpty Tests

    [Fact]
    public void IsNullOrEmpty_WithNullCollection_ShouldReturnTrue()
    {
        // Arrange
        List<int>? nullList = null;

        // Act
        var result = nullList.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_WithEmptyCollection_ShouldReturnTrue()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        var result = emptyList.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_WithNonEmptyCollection_ShouldReturnFalse()
    {
        // Arrange
        var nonEmptyList = new List<int> { 1, 2, 3 };

        // Act
        var result = nonEmptyList.IsNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region AddIfNotContains Tests

    [Fact]
    public void AddIfNotContains_Item_WhenItemNotInCollection_ShouldAddAndReturnTrue()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana" };
        const string itemToAdd = "cherry";

        // Act
        var result = collection.AddIfNotContains(itemToAdd);

        // Assert
        Assert.True(result);
        Assert.Contains(itemToAdd, collection);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void AddIfNotContains_Item_WhenItemAlreadyInCollection_ShouldNotAddAndReturnFalse()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana" };
        var itemToAdd = "banana";

        // Act
        var result = collection.AddIfNotContains(itemToAdd);

        // Assert
        Assert.False(result);
        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void AddIfNotContains_ItemCollection_ShouldAddOnlyNewItems()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana" };
        var itemsToAdd = new List<string> { "banana", "cherry", "date" };

        // Act
        var addedItems = collection.AddIfNotContains(itemsToAdd).ToList();

        // Assert
        Assert.Equal(2, addedItems.Count);
        Assert.Contains("cherry", addedItems);
        Assert.Contains("date", addedItems);
        Assert.DoesNotContain("banana", addedItems);

        Assert.Equal(4, collection.Count);
        Assert.Contains("cherry", collection);
        Assert.Contains("date", collection);
    }

    [Fact]
    public void AddIfNotContains_WithPredicate_WhenPredicateNotMatched_ShouldAddAndReturnTrue()
    {
        // Arrange
        var collection = new List<Person>
        {
            new Person { Id = 1, Name = "John" },
            new Person { Id = 2, Name = "Alice" }
        };

        // Act
        var result = collection.AddIfNotContains(p => p.Id == 3, () => new Person { Id = 3, Name = "Bob" });

        // Assert
        Assert.True(result);
        Assert.Equal(3, collection.Count);
        Assert.Contains(collection, p => p is { Id: 3, Name: "Bob" });
    }

    [Fact]
    public void AddIfNotContains_WithPredicate_WhenPredicateMatched_ShouldNotAddAndReturnFalse()
    {
        // Arrange
        var collection = new List<Person>
        {
            new Person { Id = 1, Name = "John" },
            new Person { Id = 2, Name = "Alice" }
        };

        // Act
        var result = collection.AddIfNotContains(p => p.Id == 2, () => new Person { Id = 2, Name = "Different Alice" });

        // Assert
        Assert.False(result);
        Assert.Equal(2, collection.Count);
        Assert.DoesNotContain(collection, p => p.Name == "Different Alice");
    }

    #endregion

    #region RemoveAll Tests

    [Fact]
    public void RemoveAll_WithCollection_ShouldRemoveSpecifiedItems()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana", "cherry", "date" };
        var itemsToRemove = new List<string> { "banana", "cherry" };

        // Act
        collection.RemoveAll(itemsToRemove);

        // Assert
        Assert.Equal(2, collection.Count);
        Assert.Contains("apple", collection);
        Assert.Contains("date", collection);
        Assert.DoesNotContain("banana", collection);
        Assert.DoesNotContain("cherry", collection);
    }

    #endregion

    #region ToDataTable Tests

    [Fact]
    public void ToDataTable_ShouldCreateCorrectColumns()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John" },
            new Person { Id = 2, Name = "Alice" }
        };

        // Act
        var dataTable = persons.ToDataTable();

        // Assert
        Assert.Equal(2, dataTable.Columns.Count);
        Assert.Equal("Id", dataTable.Columns[0].ColumnName);
        Assert.Equal(typeof(int), dataTable.Columns[0].DataType);
        Assert.Equal("Name", dataTable.Columns[1].ColumnName);
        Assert.Equal(typeof(string), dataTable.Columns[1].DataType);
    }

    [Fact]
    public void ToDataTable_ShouldPopulateRows()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John" },
            new Person { Id = 2, Name = "Alice" }
        };

        // Act
        var dataTable = persons.ToDataTable();

        // Assert
        Assert.Equal(2, dataTable.Rows.Count);

        Assert.Equal(1, dataTable.Rows[0]["Id"]);
        Assert.Equal("John", dataTable.Rows[0]["Name"]);

        Assert.Equal(2, dataTable.Rows[1]["Id"]);
        Assert.Equal("Alice", dataTable.Rows[1]["Name"]);
    }

    [Fact]
    public void ToDataTable_WithNullValues_ShouldUseDBNull()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = null }
        };

        // Act
        var dataTable = persons.ToDataTable();

        // Assert
        Assert.Equal(1, dataTable.Rows.Count);
        Assert.Equal(DBNull.Value, dataTable.Rows[0]["Name"]);
    }

    #endregion

    #region ToPaged Tests

    [Fact]
    public void ToPaged_ShouldReturnCorrectPage()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 100).ToList();

        // Act
        var page1 = numbers.ToPaged().ToList();
        var page5 = numbers.ToPaged(5).ToList();

        // Assert
        Assert.Equal(10, page1.Count);
        Assert.Equal(1, page1[0]);
        Assert.Equal(10, page1[9]);

        Assert.Equal(10, page5.Count);
        Assert.Equal(41, page5[0]);
        Assert.Equal(50, page5[9]);
    }

    [Fact]
    public void ToPaged_WithNegativePageNumber_ShouldUseDefaultPageNumber()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 100).ToList();

        // Act
        var result = numbers.ToPaged(-1).ToList();

        // Assert
        Assert.Equal(10, result.Count);
        Assert.Equal(1, result[0]);
    }

    [Fact]
    public void ToPaged_WithZeroPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 100).ToList();

        // Act
        var result = numbers.ToPaged(1, 0).ToList();

        // Assert
        Assert.Equal(10, result.Count);
    }

    #endregion

    #region Shuffle Tests

    [Fact]
    public void Shuffle_ShouldReturnAllItems()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 100).ToList();

        // Act
        var shuffled = numbers.Shuffle().ToList();

        // Assert
        Assert.Equal(100, shuffled.Count);
        foreach (var num in numbers)
        {
            Assert.Contains(num, shuffled);
        }
    }

    [Fact]
    public void Shuffle_ShouldChangeOrder()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 1000).ToList();

        // Act
        var shuffled = numbers.Shuffle().ToList();

        // Assert
        // Test if the shuffled sequence is different from the original
        // This is a probabilistic test - it could fail by random chance
        // but with 1000 items, the probability is extremely low
        Assert.NotEqual(numbers, shuffled);
    }

    #endregion

    #region ToCsv Tests

    [Fact]
    public void ToCsv_WithHeader_ShouldIncludeHeaderAndData()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John" },
            new Person { Id = 2, Name = "Alice" }
        };

        // Act
        var csv = persons.ToCsv();
        var lines = csv.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Equal("Id,Name", lines[0]);
        Assert.Equal("1,John", lines[1]);
        Assert.Equal("2,Alice", lines[2]);
    }

    [Fact]
    public void ToCsv_WithoutHeader_ShouldIncludeOnlyData()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John" },
            new Person { Id = 2, Name = "Alice" }
        };

        // Act
        var csv = persons.ToCsv(includeHeader: false);
        var lines = csv.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal(2, lines.Length); // Only data rows
        Assert.Equal("1,John", lines[0]);
        Assert.Equal("2,Alice", lines[1]);
    }

    [Fact]
    public void ToCsv_WithCommasInData_ShouldQuoteValues()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John, Jr." }
        };

        // Act
        var csv = persons.ToCsv();
        var lines = csv.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal(2, lines.Length);
        Assert.Equal("1,\"John, Jr.\"", lines[1]);
    }

    [Fact]
    public void ToCsv_WithNullValue_ShouldOutputEmptyString()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = null }
        };

        // Act
        var csv = persons.ToCsv();
        var lines = csv.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal(2, lines.Length);
        Assert.Equal("1,", lines[1]);
    }

    #endregion

    #region Dictionary Extension Tests

    [Fact]
    public void AddRange_ShouldAddMissingKeyValuePairs()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 }
        };

        var rangeToDictionary = new Dictionary<string, int>
        {
            { "two", 22 }, // Existing key with different value
            { "three", 3 }, // New key
            { "four", 4 } // New key
        };

        // Act
        dictionary.AddRange(rangeToDictionary);

        // Assert
        Assert.Equal(4, dictionary.Count);
        Assert.Equal(1, dictionary["one"]);
        Assert.Equal(2, dictionary["two"]); // Should keep original value
        Assert.Equal(3, dictionary["three"]);
        Assert.Equal(4, dictionary["four"]);
    }

    [Fact]
    public void GetValueOrDefault_WhenKeyExists_ShouldReturnValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 }
        };

        // Act
        var value = System.Collections.Generic.CollectionExtensions.GetValueOrDefault(dictionary, "one", 999);

        // Assert
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValueOrDefault_WhenKeyDoesNotExist_ShouldReturnDefaultValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 }
        };

        // Act
        var value = System.Collections.Generic.CollectionExtensions.GetValueOrDefault(dictionary, "three", 999);

        // Assert
        Assert.Equal(999, value);
    }

    #endregion

    // Test model class
    private class Person
    {
        public int Id { get; init; }
        public string? Name { get; init; }
    }
}