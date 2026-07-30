using Microsoft.Extensions.Logging;

namespace WindowsLocker.Core.Logging;

public class BaseLogger
{
    protected static string GetLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Critical => "FATAL",
            LogLevel.Error => "ERROR",
            LogLevel.Warning => "WARN",
            LogLevel.Information => "INFO",
            LogLevel.Debug => "DEBUG",
            LogLevel.Trace => "TRACE",
            _ => string.Empty
        };
    }
}
