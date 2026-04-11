using FluentValidation;
using Resultant.FluentValidation;

namespace Resultant.FluentValidation.Tests;

public class FluentValidationTests
{
    private record TestCommand(string Name, string Email, int Age);

    private class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Age).GreaterThan(0);
        }
    }

    [Fact]
    public void ValidateToResult_ValidInput_ShouldReturnSuccess()
    {
        var validator = new TestCommandValidator();
        var command = new TestCommand("John", "john@example.com", 25);

        var result = validator.ValidateToResult(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command, result.Value);
    }

    [Fact]
    public void ValidateToResult_InvalidInput_ShouldReturnFailureWithValidationErrors()
    {
        var validator = new TestCommandValidator();
        var command = new TestCommand("", "not-email", -1);

        var result = validator.ValidateToResult(command);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count >= 3);
        Assert.All(result.Errors, e => Assert.IsType<ValidationError>(e));
    }

    [Fact]
    public void ValidateToResult_ShouldSetPropertyName()
    {
        var validator = new TestCommandValidator();
        var command = new TestCommand("", "john@example.com", 25);

        var result = validator.ValidateToResult(command);

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ValidationError>(result.Errors[0]);
        Assert.Equal("Name", error.Property);
    }

    [Fact]
    public async Task ValidateToResultAsync_ValidInput_ShouldReturnSuccess()
    {
        var validator = new TestCommandValidator();
        var command = new TestCommand("John", "john@example.com", 25);

        var result = await validator.ValidateToResultAsync(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateToResultAsync_InvalidInput_ShouldReturnFailure()
    {
        var validator = new TestCommandValidator();
        var command = new TestCommand("", "", 0);

        var result = await validator.ValidateToResultAsync(command);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ResultValidate_OnSuccess_ShouldValidateValue()
    {
        var validator = new TestCommandValidator();
        var result = Result.Ok(new TestCommand("", "bad", -1));

        var validated = result.Validate(validator);

        Assert.False(validated.IsSuccess);
        Assert.True(validated.Errors.Count >= 3);
    }

    [Fact]
    public void ResultValidate_OnFailure_ShouldShortCircuit()
    {
        var validator = new TestCommandValidator();
        var result = Result.Fail<TestCommand>("Already failed");

        var validated = result.Validate(validator);

        Assert.False(validated.IsSuccess);
        Assert.Equal("Already failed", validated.FirstError!.Message);
    }
}
