using Microsoft.Extensions.Configuration;

namespace Teraa.Extensions.Configuration.Vault;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, Func<IConfigurationRoot, VaultConfigurationSource> sourceProvider)
    {
        var config = builder.Build();
        var source = sourceProvider(config);
        builder.Add(source);

        return builder;
    }

    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, Func<VaultConfigurationSource> sourceProvider)
    {
        var source = sourceProvider();
        builder.Add(source);

        return builder;
    }
}
