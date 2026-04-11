using FluentValidation;
using FluentValidation.Results;
using Resultant.MediatR;

namespace Resultant.MediatR.Tests;

public class ValidationBehaviorTests
{
    private record TestQuery(string Name) : IResultRequest<string>;

    private class TestQueryValidator : AbstractValidator<TestQuery>
    {
        public TestQueryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Handle_WithNoValidators_ShouldProceedToNext()
    {
        var behavior = new ValidationBehavior<TestQuery, Result<string>>(
            Enumerable.Empty<IValidator<TestQuery>>());

        var result = await behavior.Handle(
            new TestQuery("Test"),
            () => Task.FromResult(Result.Ok("Success")),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Value);
    }

    [Fact]
    public async Task Handle_WithValidInput_ShouldProceedToNext()
    {
        var behavior = new ValidationBehavior<TestQuery, Result<string>>(
            new[] { new TestQueryValidator() });

        var result = await behavior.Handle(
            new TestQuery("Valid"),
            () => Task.FromResult(Result.Ok("Success")),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Value);
    }

    [Fact]
    public async Task Handle_WithInvalidInput_ShouldReturnFailedResult()
    {
        var behavior = new ValidationBehavior<TestQuery, Result<string>>(
            new[] { new TestQueryValidator() });

        var result = await behavior.Handle(
            new TestQuery(""),
            () => Task.FromResult(Result.Ok("Should not reach")),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.HasError<ValidationError>());
    }

    [Fact]
    public async Task Handle_WithInvalidInput_ShouldNotCallNext()
    {
        var nextCalled = false;
        var behavior = new ValidationBehavior<TestQuery, Result<string>>(
            new[] { new TestQueryValidator() });

        await behavior.Handle(
            new TestQuery(""),
            () => { nextCalled = true; return Task.FromResult(Result.Ok("X")); },
            CancellationToken.None);

        Assert.False(nextCalled);
    }
}
