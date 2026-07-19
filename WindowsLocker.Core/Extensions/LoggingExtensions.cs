using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using WindowsLocker.Core.Logging;

namespace WindowsLocker.Core.Extensions;

public static class LoggingExtensions
{
    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddFileLogger()
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>()
            );
            LoggerProviderOptions.RegisterProviderOptions<FileLoggerOptions, FileLoggerProvider>(builder.Services);
            return builder;
        }

        public ILoggingBuilder AddFileLogger(Action<FileLoggerOptions> configure)
        {
            builder.AddFileLogger();
            builder.Services.Configure(configure);
            return builder;
        }

        public ILoggingBuilder AddConsoleLogger()
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <ConsoleLoggerOptions, ConsoleLoggerProvider>(builder.Services);

            return builder;
        }
        
        public ILoggingBuilder AddConsoleLogger(Action<ConsoleLoggerOptions> configure)
        {
            builder.AddConsoleLogger();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
