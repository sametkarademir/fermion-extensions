using System.Linq.Expressions;

namespace Fermion.Extensions.Linq;

/// <summary>
/// Provides extension methods for LINQ queries.
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    /// Filters a queryable collection based on a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to filter.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="predicate">The predicate to apply to the queryable collection.</param>
    /// <returns>The filtered queryable collection.</returns>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Filters an enumerable collection based on a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable collection.</typeparam>
    /// <param name="source">The enumerable collection to filter.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="predicate">The predicate to apply to the enumerable collection.</param>
    /// <returns>The filtered enumerable collection.</returns>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Orders a queryable collection based on a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to order.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="keySelector">The key selector to apply to the queryable collection.</param>
    /// <param name="ascending">Whether to order the collection in ascending or descending order.</param>
    /// <returns>The ordered queryable collection.</returns>
    public static IQueryable<T> OrderByIf<T, TKey>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, TKey>> keySelector,
        bool ascending = true)
    {
        if (!condition) return query;
        return ascending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);
    }

    /// <summary>
    /// Orders a queryable collection based on a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to order.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="keySelector">The key selector to apply to the queryable collection.</param>
    /// <param name="ascending">Whether to order the collection in ascending or descending order.</param>
    /// <returns>The ordered queryable collection.</returns>
    public static IQueryable<T> ThenByIf<T, TKey>(
        this IOrderedQueryable<T> query,
        bool condition,
        Expression<Func<T, TKey>> keySelector,
        bool ascending = true)
    {
        if (!condition) return query;
        return ascending ? query.ThenBy(keySelector) : query.ThenByDescending(keySelector);
    }

    /// <summary>
    /// Skips a specified number of elements from the queryable collection if a condition is met.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to skip elements from.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="count">The number of elements to skip.</param>
    /// <returns>The queryable collection with the specified number of elements skipped.</returns>
    public static IQueryable<T> SkipIf<T>(
        this IQueryable<T> query,
        bool condition,
        int count)
    {
        return condition ? query.Skip(count) : query;
    }

    /// <summary>
    /// Takes a specified number of elements from the queryable collection if a condition is met.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to take elements from.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="count">The number of elements to take.</param>
    /// <returns>The queryable collection with the specified number of elements taken.</returns>
    public static IQueryable<T> TakeIf<T>(
        this IQueryable<T> query,
        bool condition,
        int count)
    {
        return condition ? query.Take(count) : query;
    }

    /// <summary>
    /// Selects elements from the queryable collection based on a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection to select elements from.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="selector">The selector to apply to the queryable collection.</param>
    /// <param name="alternativeSelector">The alternative selector to apply to the queryable collection if the condition is not met.</param>
    /// <returns>The selected elements from the queryable collection.</returns>
    public static IQueryable<TResult> SelectIf<T, TResult>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, TResult>> alternativeSelector)
    {
        return condition ? query.Select(selector) : query.Select(alternativeSelector);
    }
}