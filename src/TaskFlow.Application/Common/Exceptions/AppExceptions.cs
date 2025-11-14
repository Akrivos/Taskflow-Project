using System;
using System.Collections.Generic;

namespace TaskFlow.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message)
        {
        }
    }

    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.")
        {
        }
    }

    public sealed class AppValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public AppValidationException(IDictionary<string, string[]> errors)
            : base("Validation failed.")
        {
            Errors = errors;
        }
    }

    public sealed class ForbiddenAccessException : AppException
    {
        public ForbiddenAccessException(string? message = null)
            : base(message ?? "Access denied.")
        {
        }
    }

    public sealed class ConflictException : AppException
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}