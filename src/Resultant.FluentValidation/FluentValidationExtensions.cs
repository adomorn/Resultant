using FluentValidation;
using FluentValidation.Results;

namespace Resultant.FluentValidation;

public static class FluentValidationExtensions
{
    public static Result<T> ValidateToResult<T>(this IValidator<T> validator, T instance)
    {
        var validationResult = validator.Validate(instance);
        return ToResult(validationResult, instance);
    }

    public static async Task<Result<T>> ValidateToResultAsync<T>(
        this IValidator<T> validator, T instance, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(instance, cancellationToken);
        return ToResult(validationResult, instance);
    }

    private static Result<T> ToResult<T>(ValidationResult validationResult, T instance)
    {
        if (validationResult.IsValid)
            return Result.Ok(instance);

        var errors = validationResult.Errors
            .Select(f => new ValidationError(
                f.ErrorMessage,
                f.ErrorCode ?? "Validation",
                f.PropertyName))
            .ToArray();

        return Result.Fail<T>(errors);
    }
}
