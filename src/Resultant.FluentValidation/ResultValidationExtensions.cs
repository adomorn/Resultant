using FluentValidation;

namespace Resultant.FluentValidation;

public static class ResultValidationExtensions
{
    public static Result<T> Validate<T>(this Result<T> result, IValidator<T> validator)
    {
        if (result.IsFailure)
            return result;

        return validator.ValidateToResult(result.Value);
    }

    public static async Task<Result<T>> ValidateAsync<T>(
        this Task<Result<T>> resultTask, IValidator<T> validator, CancellationToken cancellationToken = default)
    {
        var result = await resultTask;

        if (result.IsFailure)
            return result;

        return await validator.ValidateToResultAsync(result.Value, cancellationToken);
    }
}
