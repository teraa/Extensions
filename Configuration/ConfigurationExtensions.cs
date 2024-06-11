using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Teraa.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static TOptions GetValidatedRequiredOptions<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null,
        string? configSectionPath = null) where TOptions : class
    {
        configSectionPath ??= Utils.GetSectionPathFromType(typeof(TOptions));

        var options = configuration
            .GetRequiredSection(configSectionPath)
            .Get<TOptions>();

        Debug.Assert(options is not null);

        Validate(options, configSectionPath, validators);

        return options;
    }

    public static TOptions? GetValidatedOptions<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null,
        string? configSectionPath = null) where TOptions : class
    {
        configSectionPath ??= Utils.GetSectionPathFromType(typeof(TOptions));

        var section = configuration.GetSection(configSectionPath);
        if (!section.Exists())
            return default;

        var options = section.Get<TOptions>();

        Debug.Assert(options is not null);

        Validate(options, configSectionPath, validators);

        return options;
    }

    public static TOptions GetValidatedOptionsOrDefault<TOptions>(
        this IConfiguration configuration,
        IEnumerable<IValidator<TOptions>>? validators = null,
        string? configSectionPath = null)
        where TOptions : class, new()
    {
        configSectionPath ??= Utils.GetSectionPathFromType(typeof(TOptions));
        var section = configuration.GetSection(configSectionPath);
        var options = new TOptions();
        section.Bind(options);

        Validate(options, configSectionPath, validators);

        return options;
    }

    private static void Validate<TOptions>(
        TOptions options,
        string configSectionPath,
        IEnumerable<IValidator<TOptions>>? validators) where TOptions : class
    {
        if (validators is null)
            return;

        var validateOptions = new FluentValidationValidateOptions<TOptions>(configSectionPath, validators);
        var result = validateOptions.Validate(configSectionPath, options);

        if (result.Failed)
            throw new OptionsValidationException(configSectionPath, typeof(TOptions), result.Failures);
    }
}
