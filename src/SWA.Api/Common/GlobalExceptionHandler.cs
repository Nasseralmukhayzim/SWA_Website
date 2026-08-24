using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Exceptions;

namespace SWA.Api.Common;

/// <summary>Maps exceptions to RFC7807 ProblemDetails/ValidationProblemDetails matching the CMS client's parsing (title/status/detail/errors + traceId).</summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        httpContext.Response.ContentType = "application/problem+json";

        switch (exception)
        {
            case ValidationException validationException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(validationException.Errors)
                {
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["traceId"] = traceId },
                }, cancellationToken);
                return true;

            case NotFoundException notFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Not found — it may have already been deleted.",
                    Status = StatusCodes.Status404NotFound,
                    Detail = notFoundException.Message,
                    Extensions = { ["traceId"] = traceId },
                }, cancellationToken);
                return true;

            default:
                logger.LogError(exception, "Unhandled exception. TraceId={TraceId}", traceId);
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Unexpected error",
                    Status = StatusCodes.Status500InternalServerError,
                    Extensions = { ["traceId"] = traceId },
                }, cancellationToken);
                return true;
        }
    }
}
