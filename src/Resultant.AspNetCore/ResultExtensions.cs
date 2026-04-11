using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace Resultant.AspNetCore;

public static class ResultExtensions
{
    // --- ActionResult extensions (for MVC controllers) ---

    public static ActionResult ToActionResult(this Resultant.IResult result)
    {
        if (result.IsSuccess)
            return new OkResult();

        var problemDetails = ResultProblemDetailsFactory.ToProblemDetails(result.Errors);
        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    public static ActionResult<T> ToActionResult<T>(this IResult<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var problemDetails = ResultProblemDetailsFactory.ToProblemDetails(result.Errors);
        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    // --- Minimal API extensions ---

    public static HttpResult ToMinimalApiResult(this Resultant.IResult result)
    {
        if (result.IsSuccess)
            return Results.Ok();

        return ToMinimalApiErrorResult(result.Errors);
    }

    public static HttpResult ToMinimalApiResult<T>(this IResult<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return ToMinimalApiErrorResult(result.Errors);
    }

    private static HttpResult ToMinimalApiErrorResult(IReadOnlyList<ResultError> errors)
    {
        if (errors.Count == 0)
            return Results.StatusCode(500);

        var firstError = errors[0];
        var statusCode = ResultProblemDetailsFactory.GetStatusCode(firstError);

        if (errors.Any(e => e is ValidationError))
        {
            var validationErrors = errors.OfType<ValidationError>()
                .GroupBy(e => e.Property)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            return Results.ValidationProblem(validationErrors);
        }

        return Results.Problem(
            detail: firstError.Message,
            statusCode: statusCode,
            title: firstError.Code);
    }
}
