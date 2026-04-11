namespace Resultant.Tests;

public class ResultAsyncExtensionsTests
{
    private static Task<Result<int>> SuccessTask(int value) => Task.FromResult(Result.Ok(value));
    private static Task<Result<int>> FailureTask() => Task.FromResult(Result.Fail<int>("Error"));

    // --- Map ---

    [Fact]
    public async Task Map_OnSuccess_ShouldTransform()
    {
        var result = await SuccessTask(10).Map(v => v * 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public async Task Map_OnFailure_ShouldNotTransform()
    {
        var result = await FailureTask().Map(v => v * 2);

        Assert.False(result.IsSuccess);
    }

    // --- Bind ---

    [Fact]
    public async Task Bind_OnSuccess_ShouldTransform()
    {
        var result = await SuccessTask(10).Bind(v => Result.Ok(v.ToString()));

        Assert.True(result.IsSuccess);
        Assert.Equal("10", result.Value);
    }

    [Fact]
    public async Task Bind_OnFailure_ShouldNotTransform()
    {
        var result = await FailureTask().Bind(v => Result.Ok(v.ToString()));

        Assert.False(result.IsSuccess);
    }

    // --- MapAsync ---

    [Fact]
    public async Task MapAsync_OnSuccess_ShouldTransform()
    {
        var result = await SuccessTask(10).MapAsync(v => Task.FromResult(v * 2));

        Assert.True(result.IsSuccess);
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public async Task MapAsync_OnFailure_ShouldNotTransform()
    {
        var result = await FailureTask().MapAsync(v => Task.FromResult(v * 2));

        Assert.False(result.IsSuccess);
    }

    // --- BindAsync ---

    [Fact]
    public async Task BindAsync_OnSuccess_ShouldTransform()
    {
        var result = await SuccessTask(10).BindAsync(v => Task.FromResult(Result.Ok(v.ToString())));

        Assert.True(result.IsSuccess);
        Assert.Equal("10", result.Value);
    }

    [Fact]
    public async Task BindAsync_OnFailure_ShouldNotTransform()
    {
        var result = await FailureTask().BindAsync(v => Task.FromResult(Result.Ok(v.ToString())));

        Assert.False(result.IsSuccess);
    }

    // --- ThenAsync ---

    [Fact]
    public async Task ThenAsync_OnSuccess_ShouldChain()
    {
        var result = await SuccessTask(10).ThenAsync(v => Task.FromResult(Result.Ok(v + 5)));

        Assert.True(result.IsSuccess);
        Assert.Equal(15, result.Value);
    }

    [Fact]
    public async Task ThenAsync_OnFailure_ShouldShortCircuit()
    {
        var result = await FailureTask().ThenAsync(v => Task.FromResult(Result.Ok(v + 5)));

        Assert.False(result.IsSuccess);
    }

    // --- Tap ---

    [Fact]
    public async Task Tap_OnSuccess_ShouldExecuteAction()
    {
        var sideEffect = 0;
        var result = await SuccessTask(42).Tap(v => sideEffect = v);

        Assert.Equal(42, sideEffect);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Tap_OnFailure_ShouldNotExecute()
    {
        var sideEffect = 0;
        var result = await FailureTask().Tap(v => sideEffect = v);

        Assert.Equal(0, sideEffect);
        Assert.False(result.IsSuccess);
    }

    // --- TapAsync ---

    [Fact]
    public async Task TapAsync_OnSuccess_ShouldExecuteAction()
    {
        var sideEffect = 0;
        var result = await SuccessTask(42).TapAsync(v => { sideEffect = v; return Task.CompletedTask; });

        Assert.Equal(42, sideEffect);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_OnFailure_ShouldNotExecute()
    {
        var sideEffect = 0;
        var result = await FailureTask().TapAsync(v => { sideEffect = v; return Task.CompletedTask; });

        Assert.Equal(0, sideEffect);
        Assert.False(result.IsSuccess);
    }

    // --- Ensure ---

    [Fact]
    public async Task Ensure_OnSuccessWithTruePredicate_ShouldKeep()
    {
        var result = await SuccessTask(10).Ensure(v => v > 5, new ConflictError("Too small", "E"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Ensure_OnSuccessWithFalsePredicate_ShouldFail()
    {
        var result = await SuccessTask(3).Ensure(v => v > 5, new ConflictError("Too small", "E"));

        Assert.False(result.IsSuccess);
    }

    // --- Match ---

    [Fact]
    public async Task Match_OnSuccess_ShouldCallOnSuccess()
    {
        var output = await SuccessTask(42).Match(v => $"ok:{v}", _ => "fail");

        Assert.Equal("ok:42", output);
    }

    [Fact]
    public async Task Match_OnFailure_ShouldCallOnFailure()
    {
        var output = await FailureTask().Match(_ => "ok", errors => $"fail:{errors[0].Message}");

        Assert.Equal("fail:Error", output);
    }

    // --- Else ---

    [Fact]
    public async Task Else_OnFailure_ShouldReturnFallback()
    {
        var result = await FailureTask().Else(_ => Result.Ok(99));

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }

    [Fact]
    public async Task ElseAsync_OnFailure_ShouldReturnFallback()
    {
        var result = await FailureTask().ElseAsync(_ => Task.FromResult(Result.Ok(99)));

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }
}
