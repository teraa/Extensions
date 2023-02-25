using Microsoft.Extensions.Configuration;

namespace Teraa.Extensions.Configuration.Vault;

public class VaultConfigurationSource : IConfigurationSource
{
    public Uri Address { get; }
    public string Token { get; }
    public string Mount { get; }
    public string Path { get; }

    public VaultConfigurationSource(Uri address, string token, string mount, string path)
    {
        Address = address;
        Token = token;
        Mount = mount;
        Path = path;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(this);
    }
}
