using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Searchable.Core.Enums;
using Searchable.Domain;

namespace Searchable.Core;

public static class SearchableExtensions
{
    /// <summary>
    /// Searches entities implementing ISearchableModel using PostgreSQL full-text search
    /// </summary>
    /// <typeparam name="T">Entity type that implements ISearchableModel</typeparam>
    /// <param name="queryable">The queryable to filter</param>
    /// <param name="searchTerm">The search term</param>
    /// <param name="language">The language for text search (defaults to English)</param>
    /// <returns>Filtered queryable with entities matching the search term</returns>
    public static IQueryable<T> FullTextSearch<T>(
        this IQueryable<T> queryable,
        string? searchTerm,
        string language = "english")
        where T : class, ISearchableEntity
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return queryable;
        }

        // Validate language parameter
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language cannot be null, empty, or whitespace.", nameof(language));
        }

        // Process search term to handle multiple words and clean input
        string processedSearchTerm = ProcessSearchTerm(searchTerm);

        // Return early if processed term is empty after cleaning
        if (string.IsNullOrWhiteSpace(processedSearchTerm))
        {
            return queryable;
        }

        return queryable.Where(e =>
            e.SearchVector.Matches(EF.Functions.PlainToTsQuery(language, processedSearchTerm)));
    }

    /// <summary>
    /// Searches entities using PostgreSQL ILIKE pattern matching for case-insensitive searches
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="queryable">The queryable to filter</param>
    /// <param name="searchTerm">The search term to match</param>
    /// <param name="searchProperty">Expression to select the property to search on</param>
    /// <param name="matchMode">The pattern matching mode (contains, starts with, ends with, exact)</param>
    /// <returns>Filtered queryable with entities matching the search pattern</returns>
    public static IQueryable<T> ILikeSearch<T>(
        this IQueryable<T> queryable,
        string? searchTerm,
        Expression<Func<T, string>> searchProperty,
        ILikeMatchModeEnum matchMode = ILikeMatchModeEnum.Contains)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return queryable;
        }

        // Clean the search term for ILIKE usage
        string cleanedSearchTerm = CleanSearchTermForILike(searchTerm);

        if (string.IsNullOrWhiteSpace(cleanedSearchTerm))
        {
            return queryable;
        }

        // Apply pattern based on match mode
        string pattern = matchMode switch
        {
            ILikeMatchModeEnum.StartsWith => $"{cleanedSearchTerm}%",
            ILikeMatchModeEnum.EndsWith => $"%{cleanedSearchTerm}",
            ILikeMatchModeEnum.Exact => cleanedSearchTerm,
            ILikeMatchModeEnum.Contains => $"%{cleanedSearchTerm}%",
            _ => $"%{cleanedSearchTerm}%"
        };

        return queryable.Where(e => EF.Functions.ILike(searchProperty.Compile()(e), pattern));
    }

    /// <summary>
    /// Searches entities using PostgreSQL ILIKE pattern matching on multiple properties
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="queryable">The queryable to filter</param>
    /// <param name="searchTerm">The search term to match</param>
    /// <param name="searchProperties">Array of expressions to select properties to search on</param>
    /// <param name="matchMode">The pattern matching mode (contains, starts with, ends with, exact)</param>
    /// <param name="useOrLogic">If true, uses OR logic between properties; if false, uses AND logic</param>
    /// <returns>Filtered queryable with entities matching the search pattern</returns>
    public static IQueryable<T> ILikeSearch<T>(
        this IQueryable<T> queryable,
        string? searchTerm,
        Expression<Func<T, string>>[] searchProperties,
        ILikeMatchModeEnum matchMode = ILikeMatchModeEnum.Contains,
        bool useOrLogic = true)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchProperties == null || searchProperties.Length == 0)
        {
            return queryable;
        }

        // Clean the search term for ILIKE usage
        string cleanedSearchTerm = CleanSearchTermForILike(searchTerm);

        if (string.IsNullOrWhiteSpace(cleanedSearchTerm))
        {
            return queryable;
        }

        // Apply pattern based on match mode
        string pattern = matchMode switch
        {
            ILikeMatchModeEnum.StartsWith => $"{cleanedSearchTerm}%",
            ILikeMatchModeEnum.EndsWith => $"%{cleanedSearchTerm}",
            ILikeMatchModeEnum.Exact => cleanedSearchTerm,
            ILikeMatchModeEnum.Contains => $"%{cleanedSearchTerm}%",
            _ => $"%{cleanedSearchTerm}%"
        };

        if (useOrLogic)
        {
            // Use OR logic - match any of the properties
            return queryable.Where(e => searchProperties.Any(prop => EF.Functions.ILike(prop.Compile()(e), pattern)));
        }
        else
        {
            // Use AND logic - match all properties
            return queryable.Where(e => searchProperties.All(prop => EF.Functions.ILike(prop.Compile()(e), pattern)));
        }
    }

    /// <summary>
    /// Cleans the search term for safe use with PostgreSQL ILIKE
    /// </summary>
    /// <param name="searchTerm">The raw search term</param>
    /// <returns>A cleaned search term safe for ILIKE usage</returns>
    internal static string CleanSearchTermForILike(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return string.Empty;
        }

        // Trim whitespace
        searchTerm = searchTerm.Trim();

        if (string.IsNullOrEmpty(searchTerm))
        {
            return string.Empty;
        }

        // Escape ILIKE special characters: % and _ are wildcards in ILIKE
        searchTerm = searchTerm.Replace("\\", "\\\\"); // Escape backslashes first
        searchTerm = searchTerm.Replace("%", "\\%");   // Escape percent signs
        searchTerm = searchTerm.Replace("_", "\\_");   // Escape underscores

        // Limit length to prevent performance issues
        const int maxSearchTermLength = 1000;
        if (searchTerm.Length > maxSearchTermLength)
        {
            searchTerm = searchTerm[..maxSearchTermLength].TrimEnd();
        }

        return searchTerm;
    }

    /// <summary>
    /// Processes the search term to ensure proper formatting for PostgreSQL full-text search
    /// </summary>
    /// <param name="searchTerm">The raw search term input</param>
    /// <returns>A processed search term safe for PostgreSQL full-text search</returns>
    internal static string ProcessSearchTerm(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return string.Empty;
        }

        // Trim whitespace
        searchTerm = searchTerm.Trim();

        // Handle empty string after trimming
        if (string.IsNullOrEmpty(searchTerm))
        {
            return string.Empty;
        }

        // Escape special PostgreSQL characters that could cause issues
        // These characters have special meaning in PostgreSQL full-text search
        char[] specialChars = ['&', '|', '!', '(', ')', '<', '>', ':', '*'];

        foreach (char specialChar in specialChars)
        {
            searchTerm = searchTerm.Replace(specialChar.ToString(), $"\\{specialChar}");
        }

        // Replace multiple consecutive spaces with single space
        while (searchTerm.Contains("  "))
        {
            searchTerm = searchTerm.Replace("  ", " ");
        }

        // Limit the length to prevent potential performance issues
        // PostgreSQL can handle long queries, but extremely long ones may cause issues
        const int maxSearchTermLength = 1000;
        if (searchTerm.Length > maxSearchTermLength)
        {
            searchTerm = searchTerm[..maxSearchTermLength].TrimEnd();
        }

        return searchTerm;
    }
}
