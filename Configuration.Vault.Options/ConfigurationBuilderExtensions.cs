using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Teraa.Extensions.Configuration.Vault.Options;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseVault(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddOptionsWithValidation<VaultOptions>();
            services.AddSingleton<VaultService>();
            services.AddSingleton<IHostedService, VaultService>(sp => sp.GetRequiredService<VaultService>());

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using var scope = serviceProvider.CreateScope();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<VaultOptions>>().Value;

                hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
                {
                });

                hostBuilder.ConfigureAppConfiguration(config =>
                    config.AddVault(() =>
                        new VaultConfigurationSource(options.Address, options.Token, options.Mount, options.Path)));
            }

            using (var sp = services.BuildServiceProvider())
            {
                using var scope = sp.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<VaultService>();
                // service.Interval = options.Interval;
            }
        });

        return hostBuilder;
    }
}
