using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace WindowsLocker.Service.Logging;

public sealed class FileLoggerProvider(IOptions<FileLoggerOptions> options) : ILoggerProvider
{
    private readonly FileLoggerOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, ILogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, (loggerName) => new FileLogger(loggerName, _options.FilePath));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
