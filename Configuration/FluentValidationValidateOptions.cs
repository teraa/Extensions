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
    private readonly IEnumerable<IValidator<TOptions>> _validators;

    public FluentValidationValidateOptions(string? name, IEnumerable<IValidator<TOptions>> validators)
    {
        Name = name;
        _validators = validators;
    }

    /// <summary>
    /// The options name.
    /// </summary>
    public string? Name { get; }


    /// <summary>
    /// Validates a specific named options instance (or all when <paramref name="name"/> is null).
    /// </summary>
    /// <param name="name">The name of the options instance being validated.</param>
    /// <param name="options">The options instance.</param>
    /// <returns>The <see cref="ValidateOptionsResult"/> result.</returns>
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // Null name is used to configure all named options.
        if (Name != null && Name != name)
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
            .Select(x => $"{typeof(TOptions).Name}:{x.Key}: [{string.Join(" ", x)}]")
            .ToList();

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}
