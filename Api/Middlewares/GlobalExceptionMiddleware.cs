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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
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

        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case ValidationException validationException:

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Validation Failed";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                var errors = validationException
                    .Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                problemDetails.Extensions["errors"] = errors;
                problemDetails.Extensions["traceId"] = context.TraceIdentifier;
                break;

            case NotFoundException notFoundException:

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                problemDetails.Title = "Not Found";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                problemDetails.Detail = notFoundException.Message;

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;

            case AccessDeniedException accessDeniedException:

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                problemDetails.Title = "Access Denied";
                problemDetails.Status = (int)HttpStatusCode.Forbidden;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                problemDetails.Detail = accessDeniedException.Message;

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;

            case AuthenticationException authenticationException:

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                problemDetails.Title = "Unauthorized";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
                problemDetails.Detail = authenticationException.Message;

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;

            default:

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                logger.LogError(exception, "Unhandled exception");

                problemDetails.Title = "An error occurred while processing your request";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                problemDetails.Detail = "An internal error occurred. Please try again later";

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                break;
        }

        
        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}
