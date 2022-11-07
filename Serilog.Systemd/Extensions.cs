using JetBrains.Annotations;
using Microsoft.Extensions.Hosting.Systemd;
using Serilog;
using Serilog.Debugging;

namespace Teraa.Extensions.Serilog.Systemd;

[PublicAPI]
public static class Extensions
{
    public static void ConfigureSystemdConsole(this LoggerConfiguration options)
    {
        options.Enrich.FromLogContext();
        
        if (SystemdHelpers.IsSystemdService())
        {
            SelfLog.Enable(x => Console.WriteLine($"<4>SERILOG: {x}"));

            options
                .Enrich.With(new SyslogSeverityEnricher())
                .WriteTo.Console(
                    outputTemplate: "<{SyslogSeverity}>{SourceContext}: {Message:j}{NewLine}");
        }
        else
        { 
            SelfLog.Enable(x => Console.WriteLine($"SERILOG: {x}"));

            options.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
    }
}