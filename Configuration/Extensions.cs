using System.Diagnostics;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
    /// <returns>The <see cref="IServiceCollection"/> for the options being configured.</returns>
    public static IServiceCollection AddBoundOptions<[MeansImplicitUse] TOptions>(
        this IServiceCollection services,
        string? optionsName = null,
        string? configSectionPath = null,
        Action<BinderOptions>? configureBinder = null)
        where TOptions : class
    {
        configSectionPath ??= GetSectionPathFromType(typeof(TOptions));

        return services
            .AddOptions<TOptions>(optionsName)
            .BindConfiguration(configSectionPath, configureBinder)
            .Services;
    }

    /// <inheritdoc cref="AddBoundOptions{TOptions}"/>
    public static IServiceCollection AddValidatedOptions<[MeansImplicitUse] TOptions>(
        this IServiceCollection services,
        string? optionsName = null,
        string? configSectionPath = null,
        Action<BinderOptions>? configureBinder = null)
        where TOptions : class
    {
        configSectionPath ??= GetSectionPathFromType(typeof(TOptions));

        var optionsBuilder = services
            .AddOptions<TOptions>(optionsName)
            .BindConfiguration(configSectionPath, configureBinder)
            .ValidateFluentValidation()
            .ValidateOnStart();

        return optionsBuilder.Services;
    }

    public static string GetSectionPathFromType(Type optionsType)
    {
        const string suffix = "Options";

        string path = optionsType.Name;

        if (path.EndsWith(suffix))
        {
            path = path[..^suffix.Length];
        }

        return path;
    }

    public static TOptions GetRequiredOptions<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null)
    {
        string path = GetSectionPathFromType(typeof(TOptions));

        var options = configuration
            .GetRequiredSection(path)
            .Get<TOptions>();

        Debug.Assert(options is { });

        if (validators is { })
        {
            Validate(options, path, validators);
        }

        return options;
    }

    public static TOptions? GetOptions<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null)
    {
        string path = GetSectionPathFromType(typeof(TOptions));

        var section = configuration.GetSection(path);
        if (!section.Exists())
            return default;

        var options = section.Get<TOptions>();

        Debug.Assert(options is { });

        if (validators is { })
        {
            Validate(options, path, validators);
        }

        return options;
    }

    public static TOptions GetOptionsOrDefault<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null)
        where TOptions : new()
    {
        string path = GetSectionPathFromType(typeof(TOptions));

        var section = configuration.GetSection(path);
        var options = new TOptions();
        section.Bind(options);

        if (validators is { })
        {
            Validate(options, path, validators);
        }

        return options;
    }

    private static void Validate<TOptions>(
        TOptions options,
        string optionsName,
        IEnumerable<IValidator<TOptions>> validators)
    {
        var context = new ValidationContext<TOptions>(options);

        var errors = validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .Select(x => $"{typeof(TOptions).Name}:{x.Key}: [{string.Join(" ", x)}]")
            .ToList();

        if (errors.Any())
            throw new OptionsValidationException(optionsName, typeof(TOptions), errors);
    }
}
