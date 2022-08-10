using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Teraa.Extensions.AspNetCore;

[PublicAPI]
public static class Extensions
{
    public static IServiceCollection AddRequestValidationBehaviour(this IServiceCollection services)
        => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
}