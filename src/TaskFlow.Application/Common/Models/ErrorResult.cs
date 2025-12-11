using TaskFlow.Application.Common.Errors;

namespace TaskFlow.Application.Common.Models;
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException("Successful result cannot contain an error.");
        }

        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException("Failed result must contain an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success()
    {
        return new(true, null);
    }
    public static Result Failure(string code, string description, ErrorType type = ErrorType.Failure)
    {
        return new(false, new Error(code, description, type));
    }

    public static Result Failure(Error error) 
    { 
        return new(false, error); 
    }
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T value)
        : base(true, null)
    {
        Value = value;
    }

    protected Result(Error error)
        : base(false, error)
    {
        Value = default;
    }

    public static Result<T> Success(T value)
    {
        return new(value);
    }

    public static Result<T> Failure(string code, string description, ErrorType type = ErrorType.Failure)
    {
        return new(new Error(code, description, type));
    }

    public static Result<T> Failure(Error error)
    {
        return new(error);
    }
}

public static class Results
{
    public static Result Success() => Result.Success();

    public static Result Failure(string code, string description, ErrorType type = ErrorType.Failure)
        => Result.Failure(code, description, type);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string code, string description, ErrorType type = ErrorType.Failure)
        => Result<T>.Failure(code, description, type);

    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}
