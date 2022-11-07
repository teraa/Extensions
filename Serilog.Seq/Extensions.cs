using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Serilog;
using Teraa.Extensions.Configuration;

namespace Teraa.Extensions.Serilog.Seq;

[PublicAPI]
public static class Extensions
{
    public static void ConfigureSeq(this LoggerConfiguration options, HostBuilderContext hostContext)
    {
        var seqOptions = hostContext.Configuration.GetOptions(new[] {new SeqOptions.Validator()});
        if (seqOptions is { })
        {
            options.WriteTo.Seq(seqOptions.ServerUrl.ToString(), apiKey: seqOptions.ApiKey);
        }
    }
}