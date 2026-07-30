using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WindowsLocker.Core.Logging;

public sealed class ConsoleLoggerProvider(IOptions<ConsoleLoggerOptions> options) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, _ => new ConsoleLogger(options));

    public void Dispose()
    {
        _loggers.Clear();
    }
}
