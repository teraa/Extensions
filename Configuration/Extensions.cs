using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Teraa.Extensions.Configuration;

[PublicAPI]
public static class Extensions
{
    /// <summary>
    /// Registers the dependency injection container to bind <typeparamref name="TOptions"/> against
    /// the <see cref="IConfiguration"/> obtained from the DI service provider. 
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="optionsName">The name of the options instance.</param>
    /// <param name="configSectionPath">The name of the configuration section to bind from.</param>
    /// <param name="configureBinder">Optional. Used to configure the <see cref="BinderOptions"/>.</param>
    /// <typeparam name="TOptions">The options type to be configured.</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddBoundOptions<[MeansImplicitUse] TOptions>(
        this IServiceCollection services,
        string? optionsName = null,
        string? configSectionPath = null,
        Action<BinderOptions>? configureBinder = null)
        where TOptions : class
    {
        if (configSectionPath is null)
        {
            const string suffix = "Options";

            configSectionPath = typeof(TOptions).Name;

            if (configSectionPath.EndsWith(suffix))
            {
                configSectionPath = configSectionPath[..^suffix.Length];
            }
        }

        return services
            .AddOptions<TOptions>(optionsName)
            .BindConfiguration(configSectionPath, configureBinder)
            .Services;
    }
}
