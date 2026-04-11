namespace Resultant;

public readonly record struct Result : IResult
{
    private readonly IReadOnlyList<ResultError>? _errors;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<ResultError> Errors => _errors ?? [];
    public ResultError? FirstError => Errors.Count > 0 ? Errors[0] : null;

    private Result(bool isSuccess, IReadOnlyList<ResultError>? errors)
    {
        IsSuccess = isSuccess;
        _errors = errors;
    }

    // --- Factory methods ---

    public static Result Ok() => new(true, null);

    public static Result Fail(ResultError error) => new(false, [error]);

    public static Result Fail(IEnumerable<ResultError> errors) =>
        new(false, errors as IReadOnlyList<ResultError> ?? errors.ToArray());

    public static Result Fail(string message, string code = "General") =>
        Fail(new ConflictError(message, code));

    public static Result<T> Ok<T>(T value) => new(value, true, null);

    public static Result<T> Fail<T>(ResultError error) => new(default!, false, [error]);

    public static Result<T> Fail<T>(IEnumerable<ResultError> errors) =>
        new(default!, false, errors as IReadOnlyList<ResultError> ?? errors.ToArray());

    public static Result<T> Fail<T>(string message, string code = "General") =>
        Fail<T>(new ConflictError(message, code));

    // --- Exception bridging ---

    public static Result Try(Action action, Func<Exception, ResultError>? errorHandler = null)
    {
        try
        {
            action();
            return Ok();
        }
        catch (Exception ex)
        {
            var error = errorHandler?.Invoke(ex)
                ?? new InfrastructureError(ex.Message, "Exception", ex);
            return Fail(error);
        }
    }

    public static Result<T> Try<T>(Func<T> func, Func<Exception, ResultError>? errorHandler = null)
    {
        try
        {
            return Ok(func());
        }
        catch (Exception ex)
        {
            var error = errorHandler?.Invoke(ex)
                ?? new InfrastructureError(ex.Message, "Exception", ex);
            return Fail<T>(error);
        }
    }

    public static async Task<Result> TryAsync(
        Func<Task> func, Func<Exception, ResultError>? errorHandler = null)
    {
        try
        {
            await func();
            return Ok();
        }
        catch (Exception ex)
        {
            var error = errorHandler?.Invoke(ex)
                ?? new InfrastructureError(ex.Message, "Exception", ex);
            return Fail(error);
        }
    }

    public static async Task<Result<T>> TryAsync<T>(
        Func<Task<T>> func, Func<Exception, ResultError>? errorHandler = null)
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            var error = errorHandler?.Invoke(ex)
                ?? new InfrastructureError(ex.Message, "Exception", ex);
            return Fail<T>(error);
        }
    }

    // --- Querying ---

    public bool HasError<TError>() where TError : ResultError =>
        Errors.Any(e => e is TError);

    // --- Implicit operators ---

    public static implicit operator bool(Result result) => result.IsSuccess;

    // --- Deconstruct ---

    public void Deconstruct(out bool isSuccess, out IReadOnlyList<ResultError> errors)
    {
        isSuccess = IsSuccess;
        errors = Errors;
    }

    // --- ToString ---

    public override string ToString() =>
        IsSuccess ? "Success" : $"Failure: {string.Join(", ", Errors.Select(e => e.Message))}";
}
