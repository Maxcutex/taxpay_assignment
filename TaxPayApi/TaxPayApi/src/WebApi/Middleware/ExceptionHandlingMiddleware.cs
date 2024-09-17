using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using TaxPayApi.Application.Common.Exceptions;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;
            ProblemDetails problemDetails;

            switch (exception)
            {
                case ValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails = new ValidationProblemDetails(validationException.Errors)
                    {
                        Title = "Validation Failed",
                        Status = response.StatusCode,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Detail = validationException.Errors.First().Value.First() // for brevity, push the first one only
                    };
                    break;

                case NotFoundException notFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Not Found",
                        Status = response.StatusCode,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                        Detail = notFoundException.Message
                    };
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Status = response.StatusCode,
                        Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                    };
                    break;

                case ForbiddenAccessException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Forbidden",
                        Status = response.StatusCode,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                    };
                    break;

                default:
                    logger.LogError(exception, "An unexpected error occurred");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails = new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = response.StatusCode,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Detail = exception.Message
                    };
                    break;
            }

            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result);
        }
    }
}
