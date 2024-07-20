using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Teraa.Extensions.Configuration;

// Equivalent to DataAnnotationValidateOptions
public class FluentValidationValidateOptions<
    [DynamicallyAccessedMembers(memberTypes:
        DynamicallyAccessedMemberTypes.PublicProperties |
        DynamicallyAccessedMemberTypes.NonPublicProperties)]
    TOptions>
    : IValidateOptions<TOptions> where TOptions : class
{
    private readonly string? _name;
    private readonly string _configSectionPath;
    private readonly IEnumerable<IValidator<TOptions>> _validators;

    /// <summary>
    /// Creates a new <see cref="FluentValidationValidateOptions{TOptions}"/>.
    /// </summary>
    /// <param name="name">Options name.</param>
    /// <param name="configSectionPath">The config section path which is bound to the options instance being validated.</param>
    /// <param name="validators">Validators to validate the options instance.</param>
    public FluentValidationValidateOptions(
        string? name,
        string configSectionPath,
        IEnumerable<IValidator<TOptions>> validators)
    {
        _name = name;
        _configSectionPath = configSectionPath;
        _validators = validators;
    }

    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // Null name is used to configure all named options.
        if (_name != null && _name != name)
        {
            // Ignored if not validating this instance.
            return ValidateOptionsResult.Skip;
        }

        // Ensure options are provided to validate against
        ArgumentNullException.ThrowIfNull(options);

        if (!_validators.Any())
            return ValidateOptionsResult.Success;

        var context = new ValidationContext<TOptions>(options);

        var errors = _validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .Select(x => $"Error validating {_configSectionPath}:{x.Key} ({string.Join(" ", x)})")
            .ToList();

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}
