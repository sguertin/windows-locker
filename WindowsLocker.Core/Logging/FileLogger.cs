using Microsoft.Extensions.Logging;
using static WindowsLocker.Core.Constants;

namespace WindowsLocker.Core.Logging;

public class FileLogger(string name, string logFilePath) : BaseLogger, ILogger
{
    private string LogFilePath => Path.Join(logFilePath, $"{APPLICATION_NAME}Logs-{DateTime.Now:yyyy-MM-dd}.txt");

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
}
