using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WindowsLocker.Core.Providers;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Core.Extensions;

public static class HostExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddWindowsLocker()
        {
            builder.Services.AddScoped<IDateService, DateService>();
            builder.Services.AddScoped<ITimeService, TimeService>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                builder.Services.AddScoped<ILockService, WindowsLockService>();
                builder.Services.AddScoped<IWorkerService, TimedWorkerService>();
                builder.Services.AddScoped<ILockStatusProvider, SystemLockStatusProvider>();
            }
            else
            {
                builder.Services.AddScoped<ILockService, MockLockService>();
                builder.Services.AddScoped<IWorkerService, MockWorkerService>();
                builder.Services.AddScoped<ILockStatusProvider, MockLockStatusProvider>();
            }
            builder.Logging.ClearProviders();
            builder.Logging.AddFileLogger(options =>
            {
                options.FilePath = AppContext.BaseDirectory;
                options.LogLevel = builder.Configuration["Logging:LogLevel:Default"] ?? "Information";
            });
            return builder;
        }

        public IHostApplicationBuilder AddWindowsLockerConsole()
        {
            builder.Services.AddScoped<IDateService, DateService>();
            builder.Services.AddScoped<ITimeService, TimeService>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                builder.Services.AddScoped<ILockService, WindowsLockService>();
                builder.Services.AddScoped<IWorkerService, TimedWorkerService>();
                builder.Services.AddScoped<ILockStatusProvider, SystemLockStatusProvider>();
            }
            else
            {
                builder.Services.AddScoped<ILockService, MockLockService>();
                builder.Services.AddScoped<IWorkerService, MockWorkerService>();
                builder.Services.AddScoped<ILockStatusProvider, MockLockStatusProvider>();
            }
            builder.Logging.ClearProviders();
            builder.Logging.AddConsoleLogger(options =>
            {
                options.LogLevel = builder.Configuration["Logging:LogLevel:Default"] ?? "Information";
            });
            return builder;
        }
    }
}