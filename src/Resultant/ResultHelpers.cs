namespace Resultant;

public static class ResultHelpers
{
    public static Result Combine(params Result[] results)
    {
        var errors = results
            .Where(r => r.IsFailure)
            .SelectMany(r => r.Errors)
            .ToArray();

        return errors.Length > 0 ? Result.Fail(errors) : Result.Ok();
    }

    public static Result<IReadOnlyList<T>> Combine<T>(params Result<T>[] results)
    {
        var errors = results
            .Where(r => r.IsFailure)
            .SelectMany(r => r.Errors)
            .ToArray();

        if (errors.Length > 0)
            return Result.Fail<IReadOnlyList<T>>(errors);

        var values = results.Select(r => r.Value).ToArray();
        return Result.Ok<IReadOnlyList<T>>(values);
    }

    public static async Task<Result> WhenAll(IEnumerable<Task<Result>> tasks)
    {
        var results = await Task.WhenAll(tasks);
        return Combine(results);
    }

    public static async Task<Result<IReadOnlyList<T>>> WhenAll<T>(IEnumerable<Task<Result<T>>> tasks)
    {
        var results = await Task.WhenAll(tasks);
        return Combine(results);
    }
}
