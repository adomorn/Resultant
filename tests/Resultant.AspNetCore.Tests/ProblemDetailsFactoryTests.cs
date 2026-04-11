using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resultant.AspNetCore;

namespace Resultant.AspNetCore.Tests;

public class ProblemDetailsFactoryTests
{
    [Fact]
    public void ValidationError_ShouldMapTo400()
    {
        var errors = new ResultError[] { new ValidationError("Invalid", "V", "Email") };
        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);

        Assert.Equal(StatusCodes.Status400BadRequest, pd.Status);
        Assert.IsType<ValidationProblemDetails>(pd);
    }

    [Fact]
    public void NotFoundError_ShouldMapTo404()
    {
        var errors = new ResultError[] { new NotFoundError("Not found", "NF", "User") };
        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);

        Assert.Equal(StatusCodes.Status404NotFound, pd.Status);
    }

    [Fact]
    public void UnauthorizedError_ShouldMapTo401()
    {
        var errors = new ResultError[] { new UnauthorizedError("Unauthorized", "Auth") };
        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);

        Assert.Equal(StatusCodes.Status401Unauthorized, pd.Status);
    }

    [Fact]
    public void ForbiddenError_ShouldMapTo403()
    {
        var errors = new ResultError[] { new ForbiddenError("Forbidden", "Forbid") };
        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);

        Assert.Equal(StatusCodes.Status403Forbidden, pd.Status);
    }

    [Fact]
    public void ConflictError_ShouldMapTo409()
    {
        var errors = new ResultError[] { new ConflictError("Conflict", "C") };
        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);

        Assert.Equal(StatusCodes.Status409Conflict, pd.Status);
    }

    [Fact]
    public void InfrastructureError_ShouldMapTo500()
    {
        var errors = new ResultError[] { new InfrastructureError("DB error", "Infra") };
        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);

        Assert.Equal(StatusCodes.Status500InternalServerError, pd.Status);
    }

    [Fact]
    public void ValidationErrors_ShouldGroupByProperty()
    {
        var errors = new ResultError[]
        {
            new ValidationError("Required", "V", "Email"),
            new ValidationError("Too short", "V", "Email"),
            new ValidationError("Required", "V", "Name"),
        };

        var pd = ResultProblemDetailsFactory.ToProblemDetails(errors);
        var vpd = Assert.IsType<ValidationProblemDetails>(pd);

        Assert.Equal(2, vpd.Errors.Count);
        Assert.Equal(2, vpd.Errors["Email"].Length);
        Assert.Single(vpd.Errors["Name"]);
    }

    [Fact]
    public void EmptyErrors_ShouldReturn500()
    {
        var pd = ResultProblemDetailsFactory.ToProblemDetails([]);

        Assert.Equal(500, pd.Status);
    }
}
