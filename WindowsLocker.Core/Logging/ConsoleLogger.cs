using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WindowsLocker.Core.Logging;

public class ConsoleLogger(IOptions<ConsoleLoggerOptions> options) : BaseLogger, ILogger 
{
    private readonly ConsoleLoggerOptions _options = options.Value;
    private LogLevel MinimumLevel => Enum.TryParse<LogLevel>(_options.LogLevel, out var minimumLevel) ? minimumLevel : LogLevel.Information; 
    
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }
    
    public bool IsEnabled(LogLevel logLevel)
    {
        return _options.Enabled && logLevel >= MinimumLevel;
    }
    
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var contents = $"[{DateTime.Now:u}][{GetLogLevel(logLevel)}]" +
                       $"\t{formatter(state, exception)}" + Environment.NewLine; 
        Console.WriteLine(contents);
    }    
}