using Microsoft.Extensions.Configuration;

namespace Teraa.Extensions.Configuration.Vault.Options;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder)
    {
        var config = builder.Build();
        var options = config.GetRequiredOptions(new[] {new VaultOptions.Validator()});
        if (!options.IsEnabled)
            return builder;

        return builder.AddVault(
            () => new VaultConfigurationSource(options.Address, options.Token, options.Mount, options.Path));
    }
}
