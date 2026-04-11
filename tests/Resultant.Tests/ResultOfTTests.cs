namespace Resultant.Tests;

public class ResultOfTTests
{
    // --- Ok / Fail ---

    [Fact]
    public void Ok_ShouldReturnSuccessResultWithValue()
    {
        var result = Result.Ok(100);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value);
    }

    [Fact]
    public void Fail_ShouldReturnFailureResult()
    {
        var result = Result.Fail<int>(new ConflictError("Error", "E"));

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal("Error", result.Errors[0].Message);
    }

    [Fact]
    public void Value_OnFailure_ShouldThrow()
    {
        var result = Result.Fail<int>("Error");

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Default_ShouldBeFailure()
    {
        var result = default(Result<int>);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }

    // --- Map ---

    [Fact]
    public void Map_ShouldTransformValueOnSuccess()
    {
        var result = Result.Ok(10);
        var mapped = result.Map(v => v * 2);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(20, mapped.Value);
    }

    [Fact]
    public void Map_ShouldNotTransformOnFailure()
    {
        var result = Result.Fail<int>("Error");
        var mapped = result.Map(v => v * 2);

        Assert.False(mapped.IsSuccess);
    }

    // --- Bind ---

    [Fact]
    public void Bind_ShouldTransformToNewResultOnSuccess()
    {
        var result = Result.Ok(10);
        var bound = result.Bind(v => Result.Ok(v.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("10", bound.Value);
    }

    [Fact]
    public void Bind_ShouldNotTransformOnFailure()
    {
        var result = Result.Fail<int>("Error");
        var bound = result.Bind(v => Result.Ok(v.ToString()));

        Assert.False(bound.IsSuccess);
    }

    [Fact]
    public void Bind_ShouldPropagateInnerFailure()
    {
        var result = Result.Ok(10);
        var bound = result.Bind<string>(_ => Result.Fail<string>("Inner fail"));

        Assert.False(bound.IsSuccess);
        Assert.Equal("Inner fail", bound.FirstError!.Message);
    }

    // --- Tap ---

    [Fact]
    public void Tap_ShouldExecuteActionOnSuccess()
    {
        var sideEffect = 0;
        var result = Result.Ok(42).Tap(v => sideEffect = v);

        Assert.Equal(42, sideEffect);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Tap_ShouldNotExecuteOnFailure()
    {
        var sideEffect = 0;
        var result = Result.Fail<int>("Error").Tap(v => sideEffect = v);

        Assert.Equal(0, sideEffect);
        Assert.False(result.IsSuccess);
    }

    // --- Ensure ---

    [Fact]
    public void Ensure_ShouldReturnSameOnPredicateTrue()
    {
        var result = Result.Ok(10).Ensure(v => v > 5, new ConflictError("Too small", "E"));

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public void Ensure_ShouldReturnFailOnPredicateFalse()
    {
        var result = Result.Ok(3).Ensure(v => v > 5, new ConflictError("Too small", "E"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Too small", result.FirstError!.Message);
    }

    [Fact]
    public void Ensure_ShouldShortCircuitOnFailure()
    {
        var result = Result.Fail<int>("Already failed")
            .Ensure(_ => false, new ConflictError("Ignored", "E"));

        Assert.Equal("Already failed", result.FirstError!.Message);
    }

    // --- Match ---

    [Fact]
    public void Match_ShouldCallOnSuccessForSuccess()
    {
        var result = Result.Ok(42);
        var output = result.Match(
            v => $"Value: {v}",
            _ => "Failed");

        Assert.Equal("Value: 42", output);
    }

    [Fact]
    public void Match_ShouldCallOnFailureForFailure()
    {
        var result = Result.Fail<int>("Error");
        var output = result.Match(
            _ => "Success",
            errors => $"Failed: {errors[0].Message}");

        Assert.Equal("Failed: Error", output);
    }

    // --- Switch ---

    [Fact]
    public void Switch_ShouldCallOnSuccessForSuccess()
    {
        var called = "";
        Result.Ok(42).Switch(
            v => called = $"ok:{v}",
            _ => called = "fail");

        Assert.Equal("ok:42", called);
    }

    [Fact]
    public void Switch_ShouldCallOnFailureForFailure()
    {
        var called = "";
        Result.Fail<int>("Error").Switch(
            _ => called = "ok",
            errors => called = $"fail:{errors[0].Message}");

        Assert.Equal("fail:Error", called);
    }

    // --- Else ---

    [Fact]
    public void Else_ShouldReturnFallbackResultOnFailure()
    {
        var result = Result.Fail<int>("Error")
            .Else(_ => Result.Ok(99));

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }

    [Fact]
    public void Else_ShouldNotCallFallbackOnSuccess()
    {
        var result = Result.Ok(42)
            .Else(_ => Result.Ok(99));

        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Else_WithValueFunc_ShouldReturnFallbackValue()
    {
        var result = Result.Fail<int>("Error")
            .Else(_ => 99);

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }

    // --- Async pipeline ---

    [Fact]
    public async Task MapAsync_ShouldTransformValueOnSuccess()
    {
        var result = Result.Ok(10);
        var mapped = await result.MapAsync(v => Task.FromResult(v * 2));

        Assert.True(mapped.IsSuccess);
        Assert.Equal(20, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_ShouldNotTransformOnFailure()
    {
        var result = Result.Fail<int>("Error");
        var mapped = await result.MapAsync(v => Task.FromResult(v * 2));

        Assert.False(mapped.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_ShouldTransformOnSuccess()
    {
        var result = Result.Ok(10);
        var bound = await result.BindAsync(v => Task.FromResult(Result.Ok(v.ToString())));

        Assert.True(bound.IsSuccess);
        Assert.Equal("10", bound.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldNotTransformOnFailure()
    {
        var result = Result.Fail<int>("Error");
        var bound = await result.BindAsync(v => Task.FromResult(Result.Ok(v * 2)));

        Assert.False(bound.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_ShouldExecuteOnSuccess()
    {
        var sideEffect = 0;
        var result = await Result.Ok(42).TapAsync(v => { sideEffect = v; return Task.CompletedTask; });

        Assert.Equal(42, sideEffect);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_ShouldNotExecuteOnFailure()
    {
        var sideEffect = 0;
        var result = await Result.Fail<int>("Error").TapAsync(v => { sideEffect = v; return Task.CompletedTask; });

        Assert.Equal(0, sideEffect);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureAsync_ShouldReturnSameOnPredicateTrue()
    {
        var result = await Result.Ok(10).EnsureAsync(
            v => Task.FromResult(v > 5),
            new ConflictError("Too small", "E"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureAsync_ShouldFailOnPredicateFalse()
    {
        var result = await Result.Ok(3).EnsureAsync(
            v => Task.FromResult(v > 5),
            new ConflictError("Too small", "E"));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ElseAsync_ShouldReturnFallbackOnFailure()
    {
        var result = await Result.Fail<int>("Error")
            .ElseAsync(_ => Task.FromResult(Result.Ok(99)));

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }

    // --- Implicit operators ---

    [Fact]
    public void ImplicitFromValue_ShouldCreateSuccessResult()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ImplicitFromError_ShouldCreateFailureResult()
    {
        Result<int> result = new ConflictError("Error", "E");

        Assert.False(result.IsSuccess);
        Assert.Equal("Error", result.FirstError!.Message);
    }

    // --- HasError ---

    [Fact]
    public void HasError_ShouldReturnTrueForMatchingType()
    {
        var result = Result.Fail<int>(new ValidationError("Bad", "V", "Name"));

        Assert.True(result.HasError<ValidationError>());
        Assert.False(result.HasError<NotFoundError>());
    }

    // --- Deconstruct ---

    [Fact]
    public void Deconstruct_ShouldReturnComponents()
    {
        var result = Result.Ok(42);
        var (isSuccess, value, errors) = result;

        Assert.True(isSuccess);
        Assert.Equal(42, value);
        Assert.Empty(errors);
    }

    // --- ToString ---

    [Fact]
    public void ToString_Success_ShouldShowValue()
    {
        var result = Result.Ok(42);
        Assert.Equal("Success(42)", result.ToString());
    }

    [Fact]
    public void ToString_Failure_ShouldShowErrors()
    {
        var result = Result.Fail<int>("Error");
        Assert.Equal("Failure: Error", result.ToString());
    }

    // --- FirstError ---

    [Fact]
    public void FirstError_ShouldReturnNullOnSuccess()
    {
        var result = Result.Ok(42);
        Assert.Null(result.FirstError);
    }

    [Fact]
    public void FirstError_ShouldReturnFirstOnFailure()
    {
        ResultError[] errors = [new ConflictError("First", "1"), new ConflictError("Second", "2")];
        var result = Result.Fail<int>(errors);

        Assert.Equal("First", result.FirstError!.Message);
    }

    // --- Select (LINQ map) ---

    [Fact]
    public void Select_ShouldWorkAsMap()
    {
        var result = Result.Ok(10).Select(v => v * 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(20, result.Value);
    }
}
