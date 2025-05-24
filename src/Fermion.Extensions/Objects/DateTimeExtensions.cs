namespace Fermion.Extensions.Objects;

/// <summary>
/// Provides extension methods for DateTime objects.
/// </summary> 
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime object to a Unix timestamp.
    /// </summary>
    /// <param name="dateTime">The DateTime object to convert.</param>
    /// <returns>The Unix timestamp.</returns>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
    }

    /// <summary>
    /// Converts a Unix timestamp to a DateTime object.
    /// </summary>
    /// <param name="timestamp">The Unix timestamp to convert.</param>
    /// <returns>The DateTime object.</returns>
    public static DateTime FromUnixTimestamp(this long timestamp)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(timestamp);
    }

    /// <summary>
    /// Returns the start of the week for a given DateTime object.
    /// </summary>
    /// <param name="dateTime">The DateTime object.</param>
    /// <param name="startOfWeek">The start of the week.</param>
    /// <returns>The start of the week.</returns>
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Returns the start of the month for a given DateTime object.
    /// </summary>
    /// <param name="dateTime">The DateTime object.</param>
    /// <returns>The start of the month.</returns>  
    public static DateTime StartOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
    }

    /// <summary>
    /// Returns the end of the month for a given DateTime object.
    /// </summary>
    /// <param name="dateTime">The DateTime object.</param>
    /// <returns>The end of the month.</returns>
    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59,
            59, 999, dateTime.Kind);
    }

    /// <summary>
    /// Checks if a given DateTime object is a weekend day.
    /// </summary>
    /// <param name="dateTime">The DateTime object.</param>
    /// <returns>True if the day is a weekend, false otherwise.</returns> 
    public static bool IsWeekend(this DateTime dateTime)
    {
        return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Checks if a given DateTime object is between two other DateTime objects.
    /// </summary>
    /// <param name="dateTime">The DateTime object.</param>
    /// <param name="startDate">The start of the range.</param>
    /// <param name="endDate">The end of the range.</param>
    /// <param name="inclusive">Whether to include the start and end dates in the range.</param>
    /// <returns>True if the DateTime object is between the start and end dates, false otherwise.</returns>
    public static bool IsBetween(this DateTime dateTime, DateTime startDate, DateTime endDate, bool inclusive = true)
    {
        return inclusive
            ? dateTime >= startDate && dateTime <= endDate
            : dateTime > startDate && dateTime < endDate;
    }
}