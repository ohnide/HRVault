namespace HRVault.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; init; }

    public string? Message { get; init; }

    public static Result Success(string? message = null)
        => new()
        {
            Succeeded = true,
            Message = message
        };

    public static Result Failure(string message)
        => new()
        {
            Succeeded = false,
            Message = message
        };
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Success(
        T data,
        string? message = null)
        => new()
        {
            Succeeded = true,
            Data = data,
            Message = message
        };

    public new static Result<T> Failure(
        string message)
        => new()
        {
            Succeeded = false,
            Message = message
        };
}