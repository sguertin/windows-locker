using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static WindowsLocker.Core.Constants;
namespace WindowsLocker.Core.Logging;

public sealed class FileLoggerProvider(IOptions<FileLoggerOptions> options, IConfiguration configuration) : ILoggerProvider
{
    private readonly string _applicationName = configuration[APPLICATION_NAME] ?? "WindowsLocker";
    private readonly int _logRetentionDays = int.Parse(configuration[LOG_RETENTION] ?? DEFAULT_LOG_RETENTION_DAYS.ToString());
    private readonly FileLoggerOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, ILogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, (loggerName) => new FileLogger(_applicationName, loggerName, _options.FilePath, _logRetentionDays));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
