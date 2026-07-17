namespace Application.Common;

public class Result<T>(bool isSuccess, T? value, string? error)
{
    public bool IsSuccess { get; } = isSuccess;
    public T? Value { get; } = value;
    public string? Error { get; } = error;
    
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }
    
    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }
}