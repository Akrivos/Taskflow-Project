using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
                AppValidationException => StatusCodes.Status400BadRequest,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,

                ValidationException => StatusCodes.Status400BadRequest,

                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                _ => StatusCodes.Status500InternalServerError
            };

            if (status >= 500)
            {
                _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);
            }
            else
            {
                _logger.LogWarning(ex, "Handled exception. TraceId: {TraceId}", traceId);
            }

            var title = status >= 500
               ? "An unexpected error occurred."
               : ex.Message;

            object? errors = ex switch
            {
                ValidationException fvEx => fvEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),

                AppValidationException appEx => appEx.Errors,

                _ => null
            };

            var problem = new Dictionary<string, object?>
            {
                ["type"] = $"https://httpstatuses.com/{status}",
                ["title"] = title,
                ["status"] = status,
                ["traceId"] = traceId
            };

            if (errors is not null)
            {
                problem["errors"] = errors;
            }
                
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = (string?)problem["type"],
                Title = (string?)problem["title"],
                Status = (int?)problem["status"],
                Extensions =
                {
                    ["traceId"] = problem["traceId"],
                    ["errors"] = problem.ContainsKey("errors") ? problem["errors"] : null
                }
            });
        }
    }
}
