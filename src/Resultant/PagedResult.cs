namespace Resultant;

public readonly record struct PagedResult<T> : IResult<IReadOnlyList<T>>
{
    private readonly Result<IReadOnlyList<T>> _inner;

    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    public bool IsSuccess => _inner.IsSuccess;
    public bool IsFailure => _inner.IsFailure;
    public IReadOnlyList<ResultError> Errors => _inner.Errors;
    public ResultError? FirstError => _inner.FirstError;
    public IReadOnlyList<T> Value => _inner.Value;

    private PagedResult(Result<IReadOnlyList<T>> inner, int currentPage, int pageSize, int totalCount)
    {
        _inner = inner;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static PagedResult<T> Create(IReadOnlyList<T> items, int currentPage, int pageSize, int totalCount) =>
        new(Result.Ok<IReadOnlyList<T>>(items), currentPage, pageSize, totalCount);

    public static PagedResult<T> Create(List<T> items, int currentPage, int pageSize, int totalCount) =>
        new(Result.Ok<IReadOnlyList<T>>(items), currentPage, pageSize, totalCount);

    public static PagedResult<T> Fail(ResultError error) =>
        new(Result.Fail<IReadOnlyList<T>>(error), 0, 0, 0);

    public static PagedResult<T> Fail(IEnumerable<ResultError> errors) =>
        new(Result.Fail<IReadOnlyList<T>>(errors), 0, 0, 0);

    public static PagedResult<T> Fail(string message, string code = "General") =>
        Fail(new ConflictError(message, code));

    public bool HasError<TError>() where TError : ResultError => _inner.HasError<TError>();
}
