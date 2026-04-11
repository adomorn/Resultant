namespace Resultant.Tests;

public class ResultHelpersTests
{
    [Fact]
    public void Combine_WithAllSuccess_ShouldReturnSuccess()
    {
        var combined = ResultHelpers.Combine(Result.Ok(), Result.Ok());

        Assert.True(combined.IsSuccess);
    }

    [Fact]
    public void Combine_WithAnyFailure_ShouldReturnFailure()
    {
        var combined = ResultHelpers.Combine(Result.Ok(), Result.Fail("Error"));

        Assert.False(combined.IsSuccess);
    }

    [Fact]
    public void Combine_ShouldAggregateAllErrors()
    {
        var combined = ResultHelpers.Combine(
            Result.Fail("Error1"),
            Result.Ok(),
            Result.Fail("Error2"));

        Assert.Equal(2, combined.Errors.Count);
    }

    [Fact]
    public void CombineT_WithAllSuccess_ShouldReturnAllValues()
    {
        var combined = ResultHelpers.Combine(Result.Ok(1), Result.Ok(2), Result.Ok(3));

        Assert.True(combined.IsSuccess);
        Assert.Equal(3, combined.Value.Count);
        Assert.Equal(1, combined.Value[0]);
        Assert.Equal(2, combined.Value[1]);
        Assert.Equal(3, combined.Value[2]);
    }

    [Fact]
    public void CombineT_WithAnyFailure_ShouldReturnAggregatedErrors()
    {
        var combined = ResultHelpers.Combine(
            Result.Ok(1),
            Result.Fail<int>("Error1"),
            Result.Fail<int>("Error2"));

        Assert.False(combined.IsSuccess);
        Assert.Equal(2, combined.Errors.Count);
    }

    [Fact]
    public async Task WhenAll_WithAllSuccess_ShouldReturnSuccess()
    {
        var tasks = new List<Task<Result>>
        {
            Task.FromResult(Result.Ok()),
            Task.FromResult(Result.Ok())
        };

        var combined = await ResultHelpers.WhenAll(tasks);

        Assert.True(combined.IsSuccess);
    }

    [Fact]
    public async Task WhenAll_WithAnyFailure_ShouldReturnFailure()
    {
        var tasks = new List<Task<Result>>
        {
            Task.FromResult(Result.Ok()),
            Task.FromResult(Result.Fail("Error"))
        };

        var combined = await ResultHelpers.WhenAll(tasks);

        Assert.False(combined.IsSuccess);
    }

    [Fact]
    public async Task WhenAllT_WithAllSuccess_ShouldReturnAllValues()
    {
        var tasks = new List<Task<Result<int>>>
        {
            Task.FromResult(Result.Ok(1)),
            Task.FromResult(Result.Ok(2))
        };

        var combined = await ResultHelpers.WhenAll(tasks);

        Assert.True(combined.IsSuccess);
        Assert.Equal(2, combined.Value.Count);
    }

    [Fact]
    public async Task WhenAllT_WithAnyFailure_ShouldAggregateErrors()
    {
        var tasks = new List<Task<Result<int>>>
        {
            Task.FromResult(Result.Ok(1)),
            Task.FromResult(Result.Fail<int>("Error"))
        };

        var combined = await ResultHelpers.WhenAll(tasks);

        Assert.False(combined.IsSuccess);
    }
}
