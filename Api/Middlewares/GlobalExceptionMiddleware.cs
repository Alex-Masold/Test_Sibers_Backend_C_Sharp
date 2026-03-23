using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middlewares;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger
    )
    {
        context.Response.ContentType = "application/problem+json";

        var statusCode = HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;

                context.Response.StatusCode = (int)statusCode;
                problemDetails.Title = "Validation Failed";
                problemDetails.Status = (int)statusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                var errors = validationException
                    .Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                problemDetails.Extensions["errors"] = errors;
                problemDetails.Extensions["traceId"] = context.TraceIdentifier;
                break;

            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;

                context.Response.StatusCode = (int)statusCode;

                problemDetails.Title = "Not Found";
                problemDetails.Status = (int)statusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                problemDetails.Detail = notFoundException.Message;

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;

            case AccessDeniedException accessDeniedException:
                statusCode = HttpStatusCode.Forbidden;

                context.Response.StatusCode = (int)statusCode;

                problemDetails.Title = "Access Denied";
                problemDetails.Status = (int)statusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                problemDetails.Detail = accessDeniedException.Message;

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;

            case AuthenticationException authenticationException:
                statusCode = HttpStatusCode.Unauthorized;

                context.Response.StatusCode = (int)statusCode;

                problemDetails.Title = "Unauthorized";
                problemDetails.Status = (int)statusCode;
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
                problemDetails.Detail = authenticationException.Message;

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;

                context.Response.StatusCode = (int)statusCode;

                logger.LogError(exception, "Unhandled exception");

                problemDetails.Title = "An error occurred while processing your request";
                problemDetails.Status = (int)statusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                problemDetails.Detail = "An internal error occurred. Please try again later";

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
    }
}
