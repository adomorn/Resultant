using FluentValidation;
using MediatR;

namespace Resultant.MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : struct
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures
            .Select(f => new ValidationError(
                f.ErrorMessage,
                f.ErrorCode ?? "Validation",
                f.PropertyName))
            .ToArray();

        // Try to create a failed Result<T> or Result
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Fail(errors);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var failMethod = typeof(Result).GetMethod(nameof(Result.Fail), 1, [typeof(IEnumerable<ResultError>)])!;
            var genericFail = failMethod.MakeGenericMethod(valueType);
            return (TResponse)genericFail.Invoke(null, [errors.AsEnumerable()])!;
        }

        // Fallback: if response is not a Result type, proceed normally
        return await next();
    }
}
