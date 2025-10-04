using System.Net;
using Ardalis.Result;
using FluentValidation.Results;
using IResult = Ardalis.Result.IResult;

namespace WebApi.Presentation.Common.Extensions;
public static class ArdalisResultsExtensions
{
    // Extended options for response customization
    public class ResponseOptions
    {
        public bool IncludeErrorDetails { get; set; } = true;
        // Remove JsonOptions as it's not supported by FastEndpoints' SendAsync
        public Dictionary<string, string> Headers { get; set; } = [];
    }

    // Synchronous mapper version with options
    public static async Task SendResponse<TResult, TResponse>(
        this IEndpoint ep,
        TResult result,
        Func<TResult, TResponse> mapper,
        ResponseOptions? options = null)
        where TResult : IResult
    {
        options ??= new ResponseOptions();

        // Apply any custom headers
        foreach (KeyValuePair<string, string> header in options.Headers)
        {
            ep.HttpContext.Response.Headers[header.Key] = header.Value;
        }

        await HandleResultStatus(ep, result,
            async () => await ep.HttpContext.Response.SendAsync(mapper(result)),
            options.IncludeErrorDetails);
    }

    // Asynchronous mapper version with options
    public static async Task SendResponseAsync<TResult, TResponse>(
        this IEndpoint ep,
        TResult result,
        Func<TResult, Task<TResponse>> asyncMapper,
        ResponseOptions? options = null)
        where TResult : IResult
    {
        options ??= new ResponseOptions();

        // Apply any custom headers
        foreach (KeyValuePair<string, string> header in options.Headers)
        {
            ep.HttpContext.Response.Headers[header.Key] = header.Value;
        }

        await HandleResultStatus(ep, result,
            async () =>
            {
                TResponse? response = await asyncMapper(result);
                await ep.HttpContext.Response.SendAsync(response);
            },
            options.IncludeErrorDetails);
    }

    // Overloaded version for Result<T> that automatically extracts the value
    public static async Task SendResponse<TValue>(
        this IEndpoint ep,
        Result<TValue> result,
        ResponseOptions? options = null)
    {
        options ??= new ResponseOptions();

        // Apply any custom headers
        foreach (KeyValuePair<string, string> header in options.Headers)
        {
            ep.HttpContext.Response.Headers[header.Key] = header.Value;
        }

        await HandleResultStatus(ep, result,
            async () => await ep.HttpContext.Response.SendAsync(result.Value),
            options.IncludeErrorDetails);
    }

    // No response data version (for void operations)
    public static async Task SendResponse<TResult>(
        this IEndpoint ep,
        TResult result,
        ResponseOptions? options = null)
        where TResult : IResult
    {
        options ??= new ResponseOptions();

        // Apply any custom headers
        foreach (KeyValuePair<string, string> header in options.Headers)
        {
            ep.HttpContext.Response.Headers[header.Key] = header.Value;
        }

        await HandleResultStatus(ep, result,
            async () => await ep.HttpContext.Response.SendNoContentAsync(),
            options.IncludeErrorDetails);
    }

    // Core method to handle all Result statuses
    private static async Task HandleResultStatus(
        IEndpoint ep,
        IResult result,
        Func<Task> successHandler,
        bool includeErrorDetails)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
                await successHandler();
                break;

            case ResultStatus.Created:
                ep.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
                await successHandler();
                break;

            case ResultStatus.NoContent:
                ep.HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                break;

            case ResultStatus.NotFound:
                List<ValidationFailure> notFoundErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("Resource", e))]
                    : [new("Resource", "The requested resource was not found")];
                await ep.HttpContext.Response.SendErrorsAsync(notFoundErrors, (int)HttpStatusCode.NotFound);
                break;

            case ResultStatus.Unauthorized:
                List<ValidationFailure> unauthorizedErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("Authorization", e))]
                    : [new("Authorization", "You are not authorized to access this resource")];
                await ep.HttpContext.Response.SendErrorsAsync(unauthorizedErrors, (int)HttpStatusCode.Unauthorized);
                break;

            case ResultStatus.Forbidden:
                List<ValidationFailure> forbiddenErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("Permission", e))]
                    : [new("Permission", "You do not have permission to access this resource")];
                await ep.HttpContext.Response.SendErrorsAsync(forbiddenErrors, (int)HttpStatusCode.Forbidden);
                break;

            case ResultStatus.Invalid:
                if (result.ValidationErrors.Any())
                {
                    result.ValidationErrors.ToList().ForEach(e =>
                        ep.ValidationFailures.Add(new(e.Identifier, e.ErrorMessage)));
                }
                else if (result.Errors.Any())
                {
                    // If we have general errors but no validation errors
                    result.Errors.ToList().ForEach(e =>
                        ep.ValidationFailures.Add(new("Validation", e)));
                }

                await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures, (int)HttpStatusCode.BadRequest);
                break;

            case ResultStatus.Conflict:
                List<ValidationFailure> conflictErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("Conflict", e))]
                    : [new("Conflict", "The request conflicts with the current state of the resource")];
                await ep.HttpContext.Response.SendErrorsAsync(conflictErrors, (int)HttpStatusCode.Conflict);
                break;

            case ResultStatus.Error:
                List<ValidationFailure> errorErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("Error", e))]
                    : [new("Error", "An error occurred processing your request")];
                await ep.HttpContext.Response.SendErrorsAsync(errorErrors, (int)HttpStatusCode.InternalServerError);
                break;

            case ResultStatus.CriticalError:
                List<ValidationFailure> criticalErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("CriticalError", e))]
                    : [new("CriticalError", "A critical error occurred processing your request")];
                await ep.HttpContext.Response.SendErrorsAsync(criticalErrors, (int)HttpStatusCode.InternalServerError);
                break;

            case ResultStatus.Unavailable:
                List<ValidationFailure> unavailableErrors = includeErrorDetails && result.Errors.Any()
                    ? [.. result.Errors.Select(e => new ValidationFailure("Unavailable", e))]
                    : [new("Unavailable", "The service is currently unavailable")];
                await ep.HttpContext.Response.SendErrorsAsync(unavailableErrors, (int)HttpStatusCode.InternalServerError);
                break;

            default:
                await ep.HttpContext.Response.SendErrorsAsync([new("Unknown", "An unknown error occurred")], (int)HttpStatusCode.InternalServerError);
                break;
        }
    }
}
