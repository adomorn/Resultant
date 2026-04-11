namespace Resultant.Tests;

public class ErrorTests
{
    [Fact]
    public void ValidationError_ShouldSetAllProperties()
    {
        var error = new ValidationError("Invalid email", "Validation.Email", "Email");

        Assert.Equal("Invalid email", error.Message);
        Assert.Equal("Validation.Email", error.Code);
        Assert.Equal("Email", error.Property);
    }

    [Fact]
    public void NotFoundError_ShouldSetEntityProperty()
    {
        var error = new NotFoundError("User not found", "NotFound", "User");

        Assert.Equal("User not found", error.Message);
        Assert.Equal("NotFound", error.Code);
        Assert.Equal("User", error.Entity);
    }

    [Fact]
    public void ConflictError_ShouldSetMessageAndCode()
    {
        var error = new ConflictError("Already exists", "Conflict");

        Assert.Equal("Already exists", error.Message);
        Assert.Equal("Conflict", error.Code);
    }

    [Fact]
    public void UnauthorizedError_ShouldSetMessageAndCode()
    {
        var error = new UnauthorizedError("Not authenticated", "Auth");

        Assert.Equal("Not authenticated", error.Message);
        Assert.Equal("Auth", error.Code);
    }

    [Fact]
    public void ForbiddenError_ShouldSetMessageAndCode()
    {
        var error = new ForbiddenError("Access denied", "Forbidden");

        Assert.Equal("Access denied", error.Message);
        Assert.Equal("Forbidden", error.Code);
    }

    [Fact]
    public void InfrastructureError_ShouldSetInnerException()
    {
        var ex = new InvalidOperationException("DB failure");
        var error = new InfrastructureError("Database error", "Infra.DB", ex);

        Assert.Equal("Database error", error.Message);
        Assert.Equal("Infra.DB", error.Code);
        Assert.Same(ex, error.Inner);
    }

    [Fact]
    public void InfrastructureError_InnerDefaultsToNull()
    {
        var error = new InfrastructureError("Timeout", "Infra.Timeout");

        Assert.Null(error.Inner);
    }

    [Fact]
    public void RecordEquality_ShouldWorkForSameValues()
    {
        var error1 = new ConflictError("Conflict", "409");
        var error2 = new ConflictError("Conflict", "409");

        Assert.Equal(error1, error2);
    }

    [Fact]
    public void RecordEquality_ShouldFailForDifferentValues()
    {
        var error1 = new ConflictError("Conflict1", "409");
        var error2 = new ConflictError("Conflict2", "409");

        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void WithExpression_ShouldCreateModifiedCopy()
    {
        var error = new ValidationError("Bad email", "Validation", "Email");
        var modified = error with { Property = "Name" };

        Assert.Equal("Name", modified.Property);
        Assert.Equal("Bad email", modified.Message);
    }

    [Fact]
    public void AllErrorTypes_ShouldBeAssignableToResultError()
    {
        ResultError[] errors =
        [
            new ValidationError("msg", "code", "prop"),
            new NotFoundError("msg", "code", "entity"),
            new ConflictError("msg", "code"),
            new UnauthorizedError("msg", "code"),
            new ForbiddenError("msg", "code"),
            new InfrastructureError("msg", "code"),
        ];

        Assert.Equal(6, errors.Length);
        Assert.All(errors, e => Assert.IsAssignableFrom<ResultError>(e));
    }

    [Fact]
    public void PatternMatching_ShouldMatchCorrectErrorType()
    {
        ResultError error = new NotFoundError("Not found", "404", "User");

        var message = error switch
        {
            NotFoundError nf => $"{nf.Entity} not found",
            ValidationError => "validation",
            _ => "other"
        };

        Assert.Equal("User not found", message);
    }
}
