using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Products.Api.Infrastructure.Errors;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        int statusCode;
        ProblemDetails problemDetails;

        if (exception is FluentValidation.ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            problemDetails = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = "Validation Failed",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = "One or more validation errors occurred.",
                Instance = httpContext.Request.Path
            };
        }
        else if (exception is InvalidOperationException || exception is ArgumentException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            
            problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Bad Request",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            
            problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Internal Server Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Detail = "An unexpected error occurred on the server.",
                Instance = httpContext.Request.Path
            };
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
