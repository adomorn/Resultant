namespace Resultant;

public readonly record struct Result<T> : IResult<T>
{
    private readonly IReadOnlyList<ResultError>? _errors;
    private readonly T _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<ResultError> Errors => _errors ?? [];
    public ResultError? FirstError => Errors.Count > 0 ? Errors[0] : null;

    public T Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException(
                    $"Cannot access Value on a failed result. Errors: {string.Join(", ", Errors.Select(e => e.Message))}");
            return _value;
        }
    }

    internal Result(T value, bool isSuccess, IReadOnlyList<ResultError>? errors)
    {
        _value = value;
        IsSuccess = isSuccess;
        _errors = errors;
    }

    // --- Functional pipeline (sync) ---

    public Result<TNew> Map<TNew>(Func<T, TNew> map) =>
        IsFailure ? new Result<TNew>(default!, false, _errors) : Result.Ok(map(Value));

    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> bind) =>
        IsFailure ? new Result<TNew>(default!, false, _errors) : bind(Value);

    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess) action(Value);
        return this;
    }

    public Result<T> Ensure(Func<T, bool> predicate, ResultError error) =>
        IsFailure ? this : predicate(Value) ? this : Result.Fail<T>(error);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<IReadOnlyList<ResultError>, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Errors);

    public void Switch(
        Action<T> onSuccess,
        Action<IReadOnlyList<ResultError>> onFailure)
    {
        if (IsSuccess) onSuccess(Value);
        else onFailure(Errors);
    }

    public Result<T> Else(Func<IReadOnlyList<ResultError>, Result<T>> fallback) =>
        IsSuccess ? this : fallback(Errors);

    public Result<T> Else(Func<IReadOnlyList<ResultError>, T> fallback) =>
        IsSuccess ? this : Result.Ok(fallback(Errors));

    // --- Functional pipeline (async) ---

    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> map) =>
        IsFailure ? new Result<TNew>(default!, false, _errors) : Result.Ok(await map(Value));

    public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> bind) =>
        IsFailure ? new Result<TNew>(default!, false, _errors) : await bind(Value);

    public async Task<Result<T>> TapAsync(Func<T, Task> action)
    {
        if (IsSuccess) await action(Value);
        return this;
    }

    public async Task<Result<T>> EnsureAsync(Func<T, Task<bool>> predicate, ResultError error) =>
        IsFailure ? this : await predicate(Value) ? this : Result.Fail<T>(error);

    public async Task<Result<T>> ElseAsync(Func<IReadOnlyList<ResultError>, Task<Result<T>>> fallback) =>
        IsSuccess ? this : await fallback(Errors);

    // --- Error querying ---

    public bool HasError<TError>() where TError : ResultError =>
        Errors.Any(e => e is TError);

    // --- LINQ support ---

    public Result<TNew> Select<TNew>(Func<T, TNew> selector) => Map(selector);

    public Result<TNew> SelectMany<TIntermediate, TNew>(
        Func<T, Result<TIntermediate>> bind,
        Func<T, TIntermediate, TNew> project)
    {
        if (IsFailure) return new Result<TNew>(default!, false, _errors);
        var intermediate = bind(Value);
        if (intermediate.IsFailure) return new Result<TNew>(default!, false, intermediate._errors);
        return Result.Ok(project(Value, intermediate.Value));
    }

    // --- Implicit operators ---

    public static implicit operator Result<T>(T value) => Result.Ok(value);

    public static implicit operator Result<T>(ResultError error) => Result.Fail<T>(error);

    // --- Deconstruct ---

    public void Deconstruct(out bool isSuccess, out T value, out IReadOnlyList<ResultError> errors)
    {
        isSuccess = IsSuccess;
        value = _value;
        errors = Errors;
    }

    // --- ToString ---

    public override string ToString() =>
        IsSuccess ? $"Success({Value})" : $"Failure: {string.Join(", ", Errors.Select(e => e.Message))}";
}
