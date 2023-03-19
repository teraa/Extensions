using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Teraa.Extensions.Configuration.Vault.Options;

public class VaultService : BackgroundService
{
    private readonly VaultConfigurationProvider _configurationProvider;
    private readonly ILogger<VaultService> _logger;

    public VaultService(VaultConfigurationProvider configurationProvider, ILogger<VaultService> logger)
    {
        _configurationProvider = configurationProvider;
        _logger = logger;
    }

    public TimeSpan Interval { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (Interval <= TimeSpan.Zero)
            return;

        var timer = new PeriodicTimer(Interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await _configurationProvider.LoadAsync(stoppingToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading configuration");
            }
        }
    }
}
