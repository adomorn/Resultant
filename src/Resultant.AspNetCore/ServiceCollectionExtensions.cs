using Microsoft.Extensions.DependencyInjection;

namespace Resultant.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IMvcBuilder AddResultant(this IMvcBuilder builder)
    {
        builder.AddMvcOptions(options =>
        {
            options.Filters.Add<TranslateResultToActionResultFilter>();
        });

        return builder;
    }
}
