using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Resultant.AspNetCore;

public class TranslateResultToActionResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            switch (objectResult.Value)
            {
                case Resultant.IResult<object> typedResult:
                    context.Result = typedResult.ToActionResult();
                    break;
                case Resultant.IResult result:
                    context.Result = result.ToActionResult();
                    break;
            }
        }

        await next();
    }
}
