namespace Resultant;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    IReadOnlyList<ResultError> Errors { get; }
    ResultError? FirstError { get; }
    bool HasError<TError>() where TError : ResultError;
}

public interface IResult<out T> : IResult
{
    T Value { get; }
}
