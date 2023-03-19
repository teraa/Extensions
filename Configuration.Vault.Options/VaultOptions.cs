using FluentValidation;
using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace Teraa.Extensions.Configuration.Vault.Options;

[UsedImplicitly]
public class VaultOptions
{
    public bool IsEnabled { get; init; }
    public Uri Address { get; init; }
    public string Token { get; init; }
    public string Mount { get; init; }
    public string Path { get; init; }
    public TimeSpan Interval { get; init; }

    public class Validator : AbstractValidator<VaultOptions>
    {
        public Validator()
        {
            When(x => x.IsEnabled, () =>
            {
                RuleFor(x => x.Address).NotEmpty();
                RuleFor(x => x.Token).NotEmpty();
                RuleFor(x => x.Mount).NotEmpty();
                RuleFor(x => x.Path).NotEmpty();
                RuleFor(x => x.Interval >= TimeSpan.Zero);
            });
        }
    }
}
