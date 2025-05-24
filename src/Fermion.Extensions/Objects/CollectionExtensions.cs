using System.Data;
using System.Text;

namespace Fermion.Extensions.Objects;

/// <summary>
/// Extension methods for Collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Checks if a collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection to check.</param>
    /// <returns>True if the collection is null or empty, false otherwise.</returns>
    public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
    {
        return source == null || source.Count <= 0;
    }

    /// <summary>
    /// Adds an item to the collection if it's not already in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="item">The item to check and add.</param>
    /// <returns>Returns True if added, returns False if not.</returns>
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    /// <summary>
    /// Adds items to the collection which are not already in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="items">The items to check and add.</param>
    /// <returns>Returns the added items.</returns>
    public static IEnumerable<T> AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        var addedItems = new List<T>();

        foreach (var item in items)
        {
            if (source.Contains(item))
            {
                continue;
            }

            source.Add(item);
            addedItems.Add(item);
        }

        return addedItems;
    }

    /// <summary>
    /// Adds an item to the collection if it's not already in the collection based on the given <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="predicate">The condition to decide if the item is already in the collection.</param>
    /// <param name="itemFactory">A factory that returns the item.</param>
    /// <returns>Returns True if added, returns False if not.</returns>
    public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
    {
        if (source.Any(predicate))
        {
            return false;
        }

        source.Add(itemFactory());
        return true;
    }


    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="items">The items to be removed from the collection.</param>
    public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Remove(item);
        }
    }

    /// <summary>
    /// Converts an IEnumerable T to a DataTable.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <returns>A DataTable representing the collection.</returns>
    public static DataTable ToDataTable<T>(this IEnumerable<T> source) where T : class
    {
        var properties = typeof(T).GetProperties();
        var dataTable = new DataTable();

        foreach (var prop in properties)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dataTable.Columns.Add(prop.Name, type);
        }

        foreach (var item in source)
        {
            var values = new object[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i].GetValue(item) ?? DBNull.Value;
            }
            dataTable.Rows.Add(values);
        }

        return dataTable;
    }

    /// <summary>
    /// Paginates an IEnumerable collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The paged collection.</returns>
    public static IEnumerable<T> ToPaged<T>(this IEnumerable<T> source, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Shuffles an IEnumerable collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <returns>The shuffled collection.</returns> 
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var random = new Random();
        return source.OrderBy(_ => random.Next());
    }

    /// <summary>
    /// Converts an IEnumerable collection to a CSV string.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="includeHeader">Whether to include the header in the CSV string.</param>
    /// <returns>The CSV string.</returns>
    public static string ToCsv<T>(this IEnumerable<T> source, bool includeHeader = true)
    {
        var properties = typeof(T).GetProperties();
        var csv = new StringBuilder();

        if (includeHeader)
        {
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));
        }

        foreach (var item in source)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                if (value == null)
                    return string.Empty;

                var stringValue = value.ToString();

                return stringValue != null && stringValue.Contains(",") ? $"\"{stringValue}\"" : stringValue;
            });

            csv.AppendLine(string.Join(",", values));
        }

        return csv.ToString();
    }

    /// <summary>
    /// Adds a range of key-value pairs to a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="range">The range of key-value pairs to add.</param>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> range)
    {
        foreach (var pair in range)
        {
            if (!dictionary.ContainsKey(pair.Key))
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// Gets the value of a key in a dictionary or a default value if the key is not present.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key to get the value of.</param>
    /// <param name="defaultValue">The default value to return if the key is not present.</param>
    /// <returns>The value of the key or the default value if the key is not present.</returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
}