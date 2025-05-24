namespace Fermion.Extensions.Objects;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToUnixTimestamp_ConvertsDateTimeToUnixTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc);
        const int expectedTimestamp = 1684146600; // 2023-05-15 10:30:00 UTC

        // Act
        var result = dateTime.ToUnixTimestamp();

        // Assert
        Assert.Equal(expectedTimestamp, result);
    }

    [Fact]
    public void FromUnixTimestamp_ConvertsUnixTimestampToDateTime()
    {
        // Arrange
        const long timestamp = 1684146600; // 2023-05-15 10:30:00 UTC
        var expectedDateTime = new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var result = timestamp.FromUnixTimestamp();

        // Assert
        Assert.Equal(expectedDateTime, result);
    }

    [Fact]
    public void StartOfWeek_ReturnsCorrectStartOfWeek_Monday()
    {
        // Arrange - Wednesday May 17, 2023
        var dateTime = new DateTime(2023, 5, 17);
        var expected = new DateTime(2023, 5, 15); // Monday, May 15, 2023

        // Act
        var result = dateTime.StartOfWeek();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void StartOfWeek_ReturnsCorrectStartOfWeek_Sunday()
    {
        // Arrange - Wednesday May 17, 2023
        var dateTime = new DateTime(2023, 5, 17);
        var expected = new DateTime(2023, 5, 14); // Sunday, May 14, 2023

        // Act
        var result = dateTime.StartOfWeek(DayOfWeek.Sunday);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void StartOfMonth_ReturnsFirstDayOfMonth()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 17, 15, 30, 45, DateTimeKind.Utc);
        var expected = new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = dateTime.StartOfMonth();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EndOfMonth_ReturnsLastDayOfMonth()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 17, 15, 30, 45, DateTimeKind.Utc);
        var expected = new DateTime(2023, 5, 31, 23, 59, 59, 999, DateTimeKind.Utc);

        // Act
        var result = dateTime.EndOfMonth();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsWeekend_ReturnsTrueForWeekend()
    {
        // Arrange
        var saturday = new DateTime(2023, 5, 20); // Saturday
        var sunday = new DateTime(2023, 5, 21); // Sunday

        // Act & Assert
        Assert.True(saturday.IsWeekend());
        Assert.True(sunday.IsWeekend());
    }

    [Fact]
    public void IsWeekend_ReturnsFalseForWeekday()
    {
        // Arrange
        var monday = new DateTime(2023, 5, 15); // Monday
        var wednesday = new DateTime(2023, 5, 17); // Wednesday
        var friday = new DateTime(2023, 5, 19); // Friday

        // Act & Assert
        Assert.False(monday.IsWeekend());
        Assert.False(wednesday.IsWeekend());
        Assert.False(friday.IsWeekend());
    }

    [Fact]
    public void IsBetween_ReturnsTrueForDateInRange_Inclusive()
    {
        // Arrange
        var startDate = new DateTime(2023, 5, 1);
        var endDate = new DateTime(2023, 5, 31);
        var testDate = new DateTime(2023, 5, 15);
        var onStartDate = new DateTime(2023, 5, 1);
        var onEndDate = new DateTime(2023, 5, 31);

        // Act & Assert
        Assert.True(testDate.IsBetween(startDate, endDate));
        Assert.True(onStartDate.IsBetween(startDate, endDate));
        Assert.True(onEndDate.IsBetween(startDate, endDate));
    }

    [Fact]
    public void IsBetween_ReturnsFalseForDateOutsideRange_Inclusive()
    {
        // Arrange
        var startDate = new DateTime(2023, 5, 1);
        var endDate = new DateTime(2023, 5, 31);
        var beforeStartDate = new DateTime(2023, 4, 30);
        var afterEndDate = new DateTime(2023, 6, 1);

        // Act & Assert
        Assert.False(beforeStartDate.IsBetween(startDate, endDate));
        Assert.False(afterEndDate.IsBetween(startDate, endDate));
    }

    [Fact]
    public void IsBetween_ReturnsTrueForDateInRange_Exclusive()
    {
        // Arrange
        var startDate = new DateTime(2023, 5, 1);
        var endDate = new DateTime(2023, 5, 31);
        var testDate = new DateTime(2023, 5, 15);

        // Act & Assert
        Assert.True(testDate.IsBetween(startDate, endDate, false));
    }

    [Fact]
    public void IsBetween_ReturnsFalseForBoundaryDates_Exclusive()
    {
        // Arrange
        var startDate = new DateTime(2023, 5, 1);
        var endDate = new DateTime(2023, 5, 31);
        var onStartDate = new DateTime(2023, 5, 1);
        var onEndDate = new DateTime(2023, 5, 31);

        // Act & Assert
        Assert.False(onStartDate.IsBetween(startDate, endDate, false));
        Assert.False(onEndDate.IsBetween(startDate, endDate, false));
    }
}