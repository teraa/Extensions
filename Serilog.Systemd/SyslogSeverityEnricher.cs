using Serilog.Core;
using Serilog.Events;

namespace Teraa.Extensions.Serilog.Systemd;

public class SyslogSeverityEnricher : ILogEventEnricher
{
    private const string s_name = "SyslogSeverity";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var value = ToSyslogSeverityCode(logEvent.Level);
        var property = propertyFactory.CreateProperty(s_name, value);
        logEvent.AddPropertyIfAbsent(property);
    }

    private static int ToSyslogSeverityCode(LogEventLevel level)
    {
        // https://datatracker.ietf.org/doc/html/rfc5424#page-11
        return level switch
        {
            LogEventLevel.Verbose => 7,
            LogEventLevel.Debug => 7,
            LogEventLevel.Information => 6,
            LogEventLevel.Warning => 4,
            LogEventLevel.Error => 3,
            LogEventLevel.Fatal => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
    }
}