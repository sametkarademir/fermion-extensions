using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Fermion.Extensions.Objects;

/// <summary>
/// Provides extension methods for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Used to simplify and beautify casting an object to a type.
    /// </summary>
    /// <typeparam name="T">Type to be casted</typeparam>
    /// <param name="obj">Object to cast</param>
    /// <returns>Cast object</returns>
    /// <example>
    /// <code>
    /// var obj = new MyClass();
    /// var derived = obj.As&lt;MyDerivedClass&gt;();
    /// 
    /// IMyInterface interfaceObj = obj.As&lt;IMyInterface&gt;();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CastAs<T>(this object obj) where T : class
    {
        return (T)obj;
    }

    /// <summary>
    /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.Type)"/> method.
    /// </summary>
    /// <param name="obj">Object to be converted</param>
    /// <typeparam name="T">Type of the target object</typeparam>
    /// <returns>Converted object</returns>
    /// <example>
    /// <code>
    /// string numText = "42";
    /// int num = numText.To&lt;int&gt;();
    /// 
    /// object doubleObj = 3.14;
    /// decimal decimalValue = doubleObj.To&lt;decimal&gt;();
    /// 
    /// string guidText = "12345678-1234-1234-1234-123456789012";
    /// Guid guidValue = guidText.To&lt;Guid&gt;();
    /// </code>
    /// </example>
    public static T ConvertTo<T>(this object obj) where T : struct
    {
        if (typeof(T) == typeof(Guid))
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString()!)!;
        }

        return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Check if an item is in a list.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <param name="list">List of items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    /// <returns>True if the item exists in the list, otherwise false</returns>
    /// <example>
    /// <code>
    /// int number = 5;
    /// bool exists = number.IsIn(1, 3, 5, 7, 9); // true
    /// 
    /// string name = "Ahmet";
    /// bool nameExists = name.IsIn("Mehmet", "Ali", "Veli"); // false
    /// </code>
    /// </example>
    public static bool ExistsInCollection<T>(this T item, params T[] list)
    {
        return list.Contains(item);
    }

    /// <summary>
    /// Check if an item is in the given enumerable.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <param name="items">Items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    /// <returns>True if the item exists in the enumerable, otherwise false</returns>
    /// <example>
    /// <code>
    /// int number = 42;
    /// List&lt;int&gt; numbers = new List&lt;int&gt; { 10, 20, 30, 42, 50 };
    /// bool exists = number.IsIn(numbers); // true
    /// 
    /// string city = "İstanbul";
    /// IEnumerable&lt;string&gt; cities = new[] { "Ankara", "İzmir", "Bursa" };
    /// bool cityExists = city.IsIn(cities); // false
    /// </code>
    /// </example>
    public static bool ExistsInCollection<T>(this T item, IEnumerable<T> items)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Can be used to conditionally perform a function
    /// on an object and return the modified or the original object.
    /// It is useful for chained calls.
    /// </summary>
    /// <param name="obj">An object</param>
    /// <param name="condition">A condition</param>
    /// <param name="func">A function that is executed only if the condition is <code>true</code></param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>
    /// Returns the modified object (by the <paramref name="func"/> if the <paramref name="condition"/> is <code>true</code>)
    /// or the original object if the <paramref name="condition"/> is <code>false</code>
    /// </returns>
    /// <example>
    /// <code>
    /// string text = "hello world";
    /// bool shouldUpperCase = true;
    /// 
    /// string result = text.If(shouldUpperCase, t => t.ToUpper()); // "HELLO WORLD"
    /// 
    /// int number = 10;
    /// bool shouldMultiply = true;
    /// bool shouldAdd = false;
    /// 
    /// int finalResult = number
    ///     .If(shouldMultiply, n => n * 2) // 20
    ///     .If(shouldAdd, n => n + 5);     // shouldAdd false 
    /// </code>
    /// </example>
    public static T DoIf<T>(this T obj, bool condition, Func<T, T> func)
    {
        if (condition)
        {
            return func(obj);
        }

        return obj;
    }

    /// <summary>
    /// Can be used to conditionally perform an action
    /// on an object and return the original object.
    /// It is useful for chained calls on the object.
    /// </summary>
    /// <param name="obj">An object</param>
    /// <param name="condition">A condition</param>
    /// <param name="action">An action that is executed only if the condition is <code>true</code></param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>
    /// Returns the original object.
    /// </returns>
    /// <example>
    /// <code>
    /// var person = new Person { Name = "Ahmet", Age = 30 };
    /// bool shouldLog = true;
    /// 
    /// person.If(shouldLog, p => Console.WriteLine($"Name: {p.Name}, Age: {p.Age}"));
    /// // Console: "Name: Ahmet, Age: 30"
    /// 
    /// var numbers = new List&lt;int&gt; { 1, 2, 3 };
    /// bool shouldAddItem = true;
    /// bool shouldClear = false;
    /// 
    /// numbers
    ///     .If(shouldAddItem, list => list.Add(4)) // adds 4 to the list
    ///     .If(shouldClear, list => list.Clear());  // shouldClear false
    /// // Result: { 1, 2, 3, 4 }
    /// </code>
    /// </example>
    public static T DoIf<T>(this T obj, bool condition, Action<T> action)
    {
        if (condition)
        {
            action(obj);
        }

        return obj;
    }
}