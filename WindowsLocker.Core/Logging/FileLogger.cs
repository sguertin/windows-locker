using Microsoft.Extensions.Logging;
using static WindowsLocker.Core.Constants;

namespace WindowsLocker.Core.Logging;

public class FileLogger(string applicationName, string name, string logFilePath, int retentionDays) : BaseLogger, ILogger
{
    private string LogFilePath => GetFilePath(DateTime.Now);

    private string GetFilePath(DateTime date)
    {
        return Path.Join(logFilePath, $"{applicationName}Logs-{date:yyyy-MM-dd}.txt");
    }
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
        new Thread(new FileThreadWorker(retentionDays, logFilePath, DateTime.Now).CleanUp)
        {
            Name = $"{applicationName}_{nameof(FileLogger)}_{name}_CleanUp",
            IsBackground = true
        }.Start();
    }


}

public class FileThreadWorker(int retentionDays, string logFilePath, DateTime now)
{
    public void CleanUp()
    {
        var retentionDate = now.AddDays(-retentionDays); // Look for files older than the retention period
        foreach (var file in Directory.GetFiles(logFilePath)
                     .Where(f => new FileInfo(f).LastWriteTime < retentionDate))
        {
            File.Delete(file);
        }
    }
}
