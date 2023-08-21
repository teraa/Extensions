using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Teraa.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static TOptions GetValidatedRequiredOptions<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null) where TOptions : class
    {
        var path = Utils.GetSectionPathFromType(typeof(TOptions));

        var options = configuration
            .GetRequiredSection(path)
            .Get<TOptions>();

        Debug.Assert(options is not null);

        Validate(options, path, validators);

        return options;
    }

    public static TOptions? GetValidatedOptions<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null) where TOptions : class
    {
        var path = Utils.GetSectionPathFromType(typeof(TOptions));

        var section = configuration.GetSection(path);
        if (!section.Exists())
            return default;

        var options = section.Get<TOptions>();

        Debug.Assert(options is not null);

        Validate(options, path, validators);

        return options;
    }

    public static TOptions GetValidatedOptionsOrDefault<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null)
        where TOptions : class, new()
    {
        var path = Utils.GetSectionPathFromType(typeof(TOptions));
        var section = configuration.GetSection(path);
        var options = new TOptions();
        section.Bind(options);

        Validate(options, path, validators);

        return options;
    }

    private static void Validate<TOptions>(
        TOptions options,
        string optionsName,
        IEnumerable<IValidator<TOptions>>? validators) where TOptions : class
    {
        if (validators is null)
            return;

        var validateOptions = new FluentValidationValidateOptions<TOptions>(optionsName, validators);
        var result = validateOptions.Validate(optionsName, options);

        if (result.Failed)
            throw new OptionsValidationException(optionsName, typeof(TOptions), result.Failures);
    }
}
