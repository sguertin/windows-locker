using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using WindowsLocker.Core.Logging;

namespace WindowsLocker.Core.Extensions;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>()
        );
        LoggerProviderOptions.RegisterProviderOptions<FileLoggerOptions, FileLoggerProvider>(builder.Services);
        return builder;
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        builder.AddFileLogger();
        builder.Services.Configure(configure);
        return builder;
    }

    public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <ConsoleLoggerOptions, ConsoleLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> configure)
    {
        builder.AddConsoleLogger();
        builder.Services.Configure(configure);
        return builder;
    }
}
