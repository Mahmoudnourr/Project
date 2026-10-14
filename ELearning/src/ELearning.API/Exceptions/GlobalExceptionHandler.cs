using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
private readonly ILogger<GlobalExceptionHandler> _logger;

public GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
{
    _logger = logger;
}

public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
{
    var (statusCode, title, errors) = exception switch
    {
        ValidationException validationException =>
        (
            StatusCodes.Status400BadRequest,
            "Validation failed",
            validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct()
                        .ToArray())
        ),

        _ =>
        (
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred",
            (IDictionary<string, string[]>?)null
        )
    };

    if (statusCode >= 500)
    {
        _logger.LogError(
            exception,
            "An unhandled exception occurred.");
    }
    else
    {
        _logger.LogWarning(
            exception,
            "A request failed with a client error.");
    }

    httpContext.Response.StatusCode = statusCode;

    var problemDetails = new ProblemDetails
    {
        Status = statusCode,
        Title = title,
        Instance = httpContext.Request.Path
    };

    if (errors is not null)
    {
        problemDetails.Extensions["errors"] = errors;
    }

    await httpContext.Response.WriteAsJsonAsync(
        problemDetails,
        cancellationToken);

    return true;
}

}
