using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Teraa.Extensions.Configuration;

/// <summary>
/// Extension methods for adding configuration related options services to the DI container via <see cref="OptionsBuilder{TOptions}"/>.
/// </summary>
public static class OptionsBuilderFluentValidationExtensions
{
    /// <summary>
    /// Register this options instance for validation using FluentValidation.
    /// </summary>
    /// <typeparam name="TOptions">The options type to be configured.</typeparam>
    /// <param name="optionsBuilder">The options builder to add the services to.</param>
    /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that additional calls can be chained.</returns>
    public static OptionsBuilder<TOptions> ValidateFluentValidation<
        [DynamicallyAccessedMembers(memberTypes:
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.NonPublicProperties)
        ]
        TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder
    ) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(sp =>
        {
            using var scope = sp.CreateScope();
            var validators = scope.ServiceProvider.GetRequiredService<IEnumerable<IValidator<TOptions>>>();
            return new FluentValidationValidateOptions<TOptions>(optionsBuilder.Name, validators);
        });

        return optionsBuilder;
    }
}
