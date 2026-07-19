using static WindowsLocker.Service.Constants;
namespace WindowsLocker.Service.Logging;

public class FileLogger(string name, string logFilePath) : ILogger
{
    private string LogFilePath => Path.Join(logFilePath, $"{APPLICATION_NAME}-{DateTime.Now:yyyy-MM-dd}.log");
    
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }
    
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
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

        var contents = $"[{DateTime.Now:u}][{APPLICATION_NAME}][{name}][{GetLogLevel(logLevel)}]" +
                      $"\t{formatter(state, exception)}" + Environment.NewLine; 
        File.AppendAllText(LogFilePath, contents);
    }

    private static string GetLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Critical => "[FATAL]",
            LogLevel.Debug => "[DEBUG]",
            LogLevel.Error => "[ERROR]",
            LogLevel.Information => "[INFO]",
            LogLevel.Trace => "[TRACE]",
            LogLevel.Warning => "[WARN]",
            _ => string.Empty
        };
    }
}
