using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using WindowsLocker.Core.Logging;

namespace WindowsLocker.Core.Extensions;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>()
        );
        builder.Services.Configure(configure);
        return builder;
    }
}
