namespace Resultant.Tests;

public class ResultTests
{
    // --- Ok / Fail ---

    [Fact]
    public void Ok_ShouldReturnSuccessResult()
    {
        var result = Result.Ok();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Null(result.FirstError);
    }

    [Fact]
    public void Fail_WithString_ShouldReturnFailureResult()
    {
        var result = Result.Fail("Error");

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal("Error", result.Errors[0].Message);
    }

    [Fact]
    public void Fail_WithError_ShouldReturnFailureResult()
    {
        var error = new NotFoundError("Not found", "404", "User");
        var result = Result.Fail(error);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.IsType<NotFoundError>(result.Errors[0]);
    }

    [Fact]
    public void Fail_WithMultipleErrors_ShouldReturnAllErrors()
    {
        ResultError[] errors = [new ConflictError("E1", "1"), new ConflictError("E2", "2")];
        var result = Result.Fail(errors);

        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void FirstError_ShouldReturnFirstError()
    {
        ResultError[] errors = [new ConflictError("First", "1"), new ConflictError("Second", "2")];
        var result = Result.Fail(errors);

        Assert.Equal("First", result.FirstError!.Message);
    }

    // --- Default struct ---

    [Fact]
    public void Default_ShouldBeFailure()
    {
        var result = default(Result);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    // --- HasError ---

    [Fact]
    public void HasError_ShouldReturnTrueForMatchingType()
    {
        var result = Result.Fail(new ValidationError("Bad", "V", "Name"));

        Assert.True(result.HasError<ValidationError>());
        Assert.False(result.HasError<NotFoundError>());
    }

    [Fact]
    public void HasError_ShouldReturnFalseForSuccess()
    {
        var result = Result.Ok();

        Assert.False(result.HasError<ValidationError>());
    }

    // --- Implicit bool ---

    [Fact]
    public void ImplicitCastToBool_ShouldBeTrueForSuccess()
    {
        Result result = Result.Ok();
        bool isSuccess = result;

        Assert.True(isSuccess);
    }

    [Fact]
    public void ImplicitCastToBool_ShouldBeFalseForFailure()
    {
        Result result = Result.Fail("Error");
        bool isSuccess = result;

        Assert.False(isSuccess);
    }

    // --- Deconstruct ---

    [Fact]
    public void Deconstruct_ShouldReturnComponents()
    {
        var result = Result.Fail("Error");
        var (isSuccess, errors) = result;

        Assert.False(isSuccess);
        Assert.Single(errors);
    }

    // --- ToString ---

    [Fact]
    public void ToString_SuccessResult_ShouldReturnSuccess()
    {
        var result = Result.Ok();
        Assert.Equal("Success", result.ToString());
    }

    [Fact]
    public void ToString_FailureResult_ShouldReturnFailureAndError()
    {
        var result = Result.Fail("Error");
        Assert.Equal("Failure: Error", result.ToString());
    }

    // --- Try ---

    [Fact]
    public void Try_SuccessfulAction_ShouldReturnOk()
    {
        var result = Result.Try(() => { });

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Try_ThrowingAction_ShouldReturnFail()
    {
        var result = Result.Try(() => throw new InvalidOperationException("boom"));

        Assert.False(result.IsSuccess);
        Assert.IsType<InfrastructureError>(result.FirstError);
        Assert.Equal("boom", result.FirstError!.Message);
    }

    [Fact]
    public void Try_WithCustomErrorHandler_ShouldUseHandler()
    {
        var result = Result.Try(
            () => throw new InvalidOperationException("boom"),
            ex => new ConflictError(ex.Message, "Custom"));

        Assert.IsType<ConflictError>(result.FirstError);
        Assert.Equal("Custom", result.FirstError!.Code);
    }

    [Fact]
    public void TryT_SuccessfulFunc_ShouldReturnOkWithValue()
    {
        var result = Result.Try(() => 42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void TryT_ThrowingFunc_ShouldReturnFail()
    {
        var result = Result.Try<int>(() => throw new Exception("fail"));

        Assert.False(result.IsSuccess);
        Assert.IsType<InfrastructureError>(result.FirstError);
    }

    // --- TryAsync ---

    [Fact]
    public async Task TryAsync_SuccessfulFunc_ShouldReturnOk()
    {
        var result = await Result.TryAsync(() => Task.CompletedTask);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TryAsync_ThrowingFunc_ShouldReturnFail()
    {
        var result = await Result.TryAsync(() => throw new Exception("async boom"));

        Assert.False(result.IsSuccess);
        Assert.Equal("async boom", result.FirstError!.Message);
    }

    [Fact]
    public async Task TryAsyncT_SuccessfulFunc_ShouldReturnOkWithValue()
    {
        var result = await Result.TryAsync(() => Task.FromResult(42));

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task TryAsyncT_ThrowingFunc_ShouldReturnFail()
    {
        var result = await Result.TryAsync<int>(() => throw new Exception("fail"));

        Assert.False(result.IsSuccess);
    }

    // --- Generic factories ---

    [Fact]
    public void OkT_ShouldReturnSuccessResultWithValue()
    {
        var result = Result.Ok(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void FailT_WithError_ShouldReturnFailure()
    {
        var result = Result.Fail<int>(new ConflictError("Error", "E"));

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void FailT_WithString_ShouldReturnFailure()
    {
        var result = Result.Fail<int>("Something failed");

        Assert.False(result.IsSuccess);
        Assert.Equal("Something failed", result.FirstError!.Message);
    }
}
