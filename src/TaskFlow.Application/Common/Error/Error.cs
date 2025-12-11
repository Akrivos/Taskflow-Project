namespace TaskFlow.Application.Common.Errors;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
    Failure
}

public sealed record Error(
    string Code,
    string Description,
    ErrorType Type);