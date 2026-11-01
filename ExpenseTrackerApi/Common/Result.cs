namespace ExpenseTrackerApi.Common;

public enum ErrorType
{
    None,
    InvalidToken,
    InvalidCredentials,
    CompromisedToken,
    UserNotFound,
    UserAlreadyExists
}

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorType? Error { get; }   

    protected Result(bool success, ErrorType? error)
    {
        IsSuccess = success;
        Error = error;
    }  
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, ErrorType? error) : base(success, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(ErrorType error) => new(false, default, error);
}