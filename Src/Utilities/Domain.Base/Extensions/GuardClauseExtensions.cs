using System.Text.RegularExpressions;
using Ardalis.GuardClauses;

namespace Domain.Base.Extensions;

public static class GuardClauseExtensions
{
    /// <summary>
    /// Validates a string input with comprehensive checks including null/whitespace, length constraints, and optional regex pattern validation.
    /// </summary>
    /// <param name="guardClause">The guard clause instance.</param>
    /// <param name="input">The string input to validate.</param>
    /// <param name="parameterName">The parameter name for error messages.</param>
    /// <param name="minLength">Minimum required length (optional).</param>
    /// <param name="maxLength">Maximum allowed length (optional).</param>
    /// <param name="pattern">Optional regex pattern for format validation.</param>
    /// <param name="patternErrorMessage">Custom error message for pattern validation failures.</param>
    /// <param name="allowNullOrEmpty">Whether null or empty strings are allowed (default: false).</param>
    /// <returns>The validated input string.</returns>
    public static string ValidString(
        this IGuardClause guardClause,
        string? input,
        string parameterName,
        int? minLength = null,
        int? maxLength = null,
        string? pattern = null,
        string? patternErrorMessage = null,
        bool allowNullOrEmpty = false)
    {
        // Check for null/empty/whitespace
        if (!allowNullOrEmpty)
        {
            Guard.Against.NullOrWhiteSpace(input, parameterName);
        }
        else if (input == null)
        {
            return string.Empty;
        }

        // Length validations
        if (minLength.HasValue)
        {
            Guard.Against.InvalidInput(input, parameterName,
                x => x.Length >= minLength.Value,
                $"{parameterName} must be at least {minLength.Value} characters long.");
        }

        if (maxLength.HasValue)
        {
            Guard.Against.InvalidInput(input, parameterName,
                x => x.Length <= maxLength.Value,
                $"{parameterName} cannot exceed {maxLength.Value} characters.");
        }

        // Pattern validation
        if (!string.IsNullOrEmpty(pattern))
        {
            Guard.Against.InvalidInput(input, parameterName,
                x => Regex.IsMatch(x, pattern),
                patternErrorMessage ?? $"{parameterName} does not match the required format.");
        }

        return input;
    }

    /// <summary>
    /// Validates an ISO country code (2 or 3 characters, uppercase letters only).
    /// </summary>
    /// <param name="guardClause">The guard clause instance.</param>
    /// <param name="input">The ISO code to validate.</param>
    /// <param name="parameterName">The parameter name for error messages.</param>
    /// <param name="length">Expected length (2 for ISO2, 3 for ISO3).</param>
    /// <returns>The normalized (uppercase) ISO code.</returns>
    public static string ValidIsoCode(
        this IGuardClause guardClause,
        string? input,
        string parameterName,
        int length)
    {
        Guard.Against.NullOrWhiteSpace(input, parameterName);

        string normalized = input.ToUpperInvariant();

        Guard.Against.InvalidInput(normalized, parameterName,
            x => x.Length == length && x.All(char.IsAsciiLetterUpper),
            $"{parameterName} must be exactly {length} uppercase letters.");

        return normalized;
    }

    /// <summary>
    /// Validates a country name with standard constraints.
    /// </summary>
    /// <param name="guardClause">The guard clause instance.</param>
    /// <param name="input">The country name to validate.</param>
    /// <param name="parameterName">The parameter name for error messages.</param>
    /// <param name="maxLength">Maximum allowed length (default: 128).</param>
    /// <returns>The validated country name.</returns>
    public static string ValidCountryName(
        this IGuardClause guardClause,
        string? input,
        string parameterName,
        int maxLength = 128)
    {
        return guardClause.ValidString(input, parameterName, minLength: 1, maxLength: maxLength);
    }

    /// <summary>
    /// Validates a currency code (3 characters, uppercase letters only).
    /// </summary>
    /// <param name="guardClause">The guard clause instance.</param>
    /// <param name="input">The currency code to validate.</param>
    /// <param name="parameterName">The parameter name for error messages.</param>
    /// <returns>The normalized (uppercase) currency code.</returns>
    public static string ValidCurrencyCode(
        this IGuardClause guardClause,
        string? input,
        string parameterName)
    {
        return guardClause.ValidIsoCode(input, parameterName, 3);
    }

    /// <summary>
    /// Validates a currency name with standard constraints.
    /// </summary>
    /// <param name="guardClause">The guard clause instance.</param>
    /// <param name="input">The currency name to validate.</param>
    /// <param name="parameterName">The parameter name for error messages.</param>
    /// <param name="maxLength">Maximum allowed length (default: 128).</param>
    /// <returns>The validated currency name.</returns>
    public static string ValidCurrencyName(
        this IGuardClause guardClause,
        string? input,
        string parameterName,
        int maxLength = 128)
    {
        return guardClause.ValidString(input, parameterName, minLength: 1, maxLength: maxLength);
    }

    /// <summary>
    /// Validates a currency symbol with standard constraints.
    /// </summary>
    /// <param name="guardClause">The guard clause instance.</param>
    /// <param name="input">The currency symbol to validate.</param>
    /// <param name="parameterName">The parameter name for error messages.</param>
    /// <param name="maxLength">Maximum allowed length (default: 8).</param>
    /// <returns>The validated currency symbol or null if input was null/empty.</returns>
    public static string? ValidCurrencySymbol(
        this IGuardClause guardClause,
        string? input,
        string parameterName,
        int maxLength = 8)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        return guardClause.ValidString(input, parameterName, maxLength: maxLength, allowNullOrEmpty: true);
    }
}
