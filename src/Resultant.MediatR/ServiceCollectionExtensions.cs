using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Resultant.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResultantMediatR(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
