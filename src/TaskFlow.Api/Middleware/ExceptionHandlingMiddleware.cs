using System.Text.Json;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Common.Exceptions;

namespace TaskFlow.Api.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;

            var status = ex switch
            {
                // δικά σου app exceptions
                AppValidationException => StatusCodes.Status400BadRequest,
                ForbiddenAccessException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                NotFoundException => StatusCodes.Status404NotFound,

                // FluentValidation
                ValidationException => StatusCodes.Status400BadRequest,

                // κλασικά .NET
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                _ => StatusCodes.Status500InternalServerError
            };

            _logger.LogError(ex, "❌ Unhandled exception. TraceId: {TraceId}", traceId);

            var title = ex switch
            {
                AppValidationException => ex.Message ?? "Validation failed.",
                ValidationException => ex.Message ?? "Validation failed.",
                ForbiddenAccessException => ex.Message ?? "Forbidden.",
                ConflictException => ex.Message ?? "Conflict.",
                NotFoundException => ex.Message ?? "Not Found.",
                KeyNotFoundException => ex.Message ?? "Resource not found.",
                UnauthorizedAccessException => ex.Message ?? "Unauthorized.",
                _ => "Unexpected error"
            };

            var errors = ex switch
            {
                ValidationException fvEx => fvEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),

                AppValidationException appEx => appEx.Errors,

                _ => null
            };

            var problem = new
            {
                type = $"https://httpstatuses.com/{status}",
                title,
                status,
                traceId,
                errors
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
