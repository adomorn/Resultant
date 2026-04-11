namespace Resultant;

/// <summary>
/// Extension methods on Task&lt;Result&lt;T&gt;&gt; enabling single-await async chaining.
/// </summary>
public static class ResultAsyncExtensions
{
    public static async Task<Result<TNew>> Map<T, TNew>(
        this Task<Result<T>> resultTask, Func<T, TNew> map)
    {
        var result = await resultTask;
        return result.Map(map);
    }

    public static async Task<Result<TNew>> Bind<T, TNew>(
        this Task<Result<T>> resultTask, Func<T, Result<TNew>> bind)
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Task<Result<T>> resultTask, Func<T, Task<TNew>> map)
    {
        var result = await resultTask;
        return await result.MapAsync(map);
    }

    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask, Func<T, Task<Result<TNew>>> bind)
    {
        var result = await resultTask;
        return await result.BindAsync(bind);
    }

    public static async Task<Result<T>> ThenAsync<T>(
        this Task<Result<T>> resultTask, Func<T, Task<Result<T>>> next)
    {
        var result = await resultTask;
        return await result.BindAsync(next);
    }

    public static async Task<Result<T>> Tap<T>(
        this Task<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    public static async Task<Result<T>> TapAsync<T>(
        this Task<Result<T>> resultTask, Func<T, Task> action)
    {
        var result = await resultTask;
        return await result.TapAsync(action);
    }

    public static async Task<Result<T>> Ensure<T>(
        this Task<Result<T>> resultTask, Func<T, bool> predicate, ResultError error)
    {
        var result = await resultTask;
        return result.Ensure(predicate, error);
    }

    public static async Task<Result<T>> EnsureAsync<T>(
        this Task<Result<T>> resultTask, Func<T, Task<bool>> predicate, ResultError error)
    {
        var result = await resultTask;
        return await result.EnsureAsync(predicate, error);
    }

    public static async Task<TResult> Match<T, TResult>(
        this Task<Result<T>> resultTask,
        Func<T, TResult> onSuccess,
        Func<IReadOnlyList<ResultError>, TResult> onFailure)
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    public static async Task Switch<T>(
        this Task<Result<T>> resultTask,
        Action<T> onSuccess,
        Action<IReadOnlyList<ResultError>> onFailure)
    {
        var result = await resultTask;
        result.Switch(onSuccess, onFailure);
    }

    public static async Task<Result<T>> Else<T>(
        this Task<Result<T>> resultTask,
        Func<IReadOnlyList<ResultError>, Result<T>> fallback)
    {
        var result = await resultTask;
        return result.Else(fallback);
    }

    public static async Task<Result<T>> ElseAsync<T>(
        this Task<Result<T>> resultTask,
        Func<IReadOnlyList<ResultError>, Task<Result<T>>> fallback)
    {
        var result = await resultTask;
        return await result.ElseAsync(fallback);
    }
}
