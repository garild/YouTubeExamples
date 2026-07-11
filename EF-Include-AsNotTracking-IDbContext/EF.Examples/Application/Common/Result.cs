namespace EF.Examples.Application.Common;

public sealed class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    private Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true);

    public static Result Fail(string error) => new(false, error);
}

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool HasData { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        HasData = true;
        Value = value;
    }

    private Result(bool isSuccess, bool hasData, string? error = null)
    {
        IsSuccess = isSuccess;
        HasData = hasData;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(value);

    public static Result<T> NoData() => new(isSuccess: true, hasData: false);

    public static Result<T> Fail(string error) => new(isSuccess: false, hasData: false, error);
}
