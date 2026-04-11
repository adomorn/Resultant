namespace Resultant.Tests;

public class PagedResultTests
{
    [Fact]
    public void Create_ShouldReturnSuccessPagedResult()
    {
        var items = new List<int> { 1, 2, 3 };
        var pagedResult = PagedResult<int>.Create(items, 1, 3, 10);

        Assert.True(pagedResult.IsSuccess);
        Assert.Equal(3, pagedResult.Value.Count);
        Assert.Equal(1, pagedResult.CurrentPage);
        Assert.Equal(3, pagedResult.PageSize);
        Assert.Equal(10, pagedResult.TotalCount);
        Assert.Equal(4, pagedResult.TotalPages);
    }

    [Fact]
    public void Create_WithIReadOnlyList_ShouldWork()
    {
        IReadOnlyList<string> items = new[] { "a", "b" };
        var pagedResult = PagedResult<string>.Create(items, 2, 2, 5);

        Assert.True(pagedResult.IsSuccess);
        Assert.Equal(2, pagedResult.Value.Count);
        Assert.Equal(3, pagedResult.TotalPages);
    }

    [Fact]
    public void Fail_WithString_ShouldReturnFailure()
    {
        var pagedResult = PagedResult<int>.Fail("Error");

        Assert.False(pagedResult.IsSuccess);
        Assert.True(pagedResult.IsFailure);
        Assert.Single(pagedResult.Errors);
    }

    [Fact]
    public void Fail_WithError_ShouldReturnFailure()
    {
        var pagedResult = PagedResult<int>.Fail(new NotFoundError("Not found", "404", "Items"));

        Assert.False(pagedResult.IsSuccess);
        Assert.IsType<NotFoundError>(pagedResult.FirstError);
    }

    [Fact]
    public void Fail_WithMultipleErrors_ShouldReturnAllErrors()
    {
        ResultError[] errors = [new ConflictError("E1", "1"), new ConflictError("E2", "2")];
        var pagedResult = PagedResult<int>.Fail(errors);

        Assert.Equal(2, pagedResult.Errors.Count);
    }

    [Fact]
    public void TotalPages_WithZeroPageSize_ShouldReturnZero()
    {
        var pagedResult = default(PagedResult<int>);

        Assert.Equal(0, pagedResult.TotalPages);
    }

    [Fact]
    public void HasError_ShouldWorkOnPagedResult()
    {
        var pagedResult = PagedResult<int>.Fail(new ValidationError("Bad", "V", "Page"));

        Assert.True(pagedResult.HasError<ValidationError>());
        Assert.False(pagedResult.HasError<NotFoundError>());
    }

    [Fact]
    public void Value_OnFailure_ShouldThrow()
    {
        var pagedResult = PagedResult<int>.Fail("Error");

        Assert.Throws<InvalidOperationException>(() => pagedResult.Value);
    }
}
