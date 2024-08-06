using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Teraa.Extensions.AspNetCore;

[PublicAPI]
public static class Extensions
{
    public static IServiceCollection AddRequestValidationBehaviour(this IServiceCollection services)
        => services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(Controllers.RequestValidationBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(MinimalApis.RequestValidationBehaviour<,>));

    public static IHttpClientBuilder AddKeyedHttpMessageHandler<THandler>(this IHttpClientBuilder builder, object key)
        where THandler : DelegatingHandler
    {
        builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, options =>
        {
            options.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(b.Services.GetRequiredKeyedService<THandler>(key)));
        });

        return builder;
    }
}
