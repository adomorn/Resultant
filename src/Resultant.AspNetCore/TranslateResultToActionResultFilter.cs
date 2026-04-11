using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Resultant.AspNetCore;

public class TranslateResultToActionResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is Resultant.IResult result)
        {
            context.Result = result.ToActionResult();
        }

        await next();
    }
}
