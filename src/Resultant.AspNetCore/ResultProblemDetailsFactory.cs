using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Resultant.AspNetCore;

public static class ResultProblemDetailsFactory
{
    public static ProblemDetails ToProblemDetails(IReadOnlyList<ResultError> errors)
    {
        if (errors.Count == 0)
            return new ProblemDetails { Status = 500, Title = "Unknown Error" };

        var firstError = errors[0];
        var statusCode = GetStatusCode(firstError);

        if (errors.Any(e => e is ValidationError))
        {
            var validationErrors = errors.OfType<ValidationError>()
                .GroupBy(e => e.Property)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            return new ValidationProblemDetails(validationErrors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
        }

        return new ProblemDetails
        {
            Status = statusCode,
            Title = firstError.Code,
            Detail = firstError.Message,
            Type = GetProblemType(statusCode)
        };
    }

    public static int GetStatusCode(ResultError error) => error switch
    {
        ValidationError => StatusCodes.Status400BadRequest,
        UnauthorizedError => StatusCodes.Status401Unauthorized,
        ForbiddenError => StatusCodes.Status403Forbidden,
        NotFoundError => StatusCodes.Status404NotFound,
        ConflictError => StatusCodes.Status409Conflict,
        InfrastructureError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };
}
