using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Teraa.Extensions.Configuration.Vault;

public class VaultConfigurationProvider : ConfigurationProvider
{
    private readonly VaultConfigurationSource _source;
    private readonly HttpClient _client;

    public VaultConfigurationProvider(VaultConfigurationSource source)
    {
        _source = source;
        _client = new HttpClient
        {
            BaseAddress = _source.Address
        };
    }

    public override void Load()
    {
        LoadAsync(default).GetAwaiter().GetResult();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"v1/{_source.Mount}/data/{_source.Path}")
        {
            Headers = {{"X-Vault-Token", _source.Token}}
        };

        using var response = await _client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return;

        using var json = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);

        var data = json.SelectElement("$.data.data");
        var dict = data?.Deserialize<Dictionary<string, string?>>();
        if (dict is null)
            return;

        Data = dict;
    }
}
