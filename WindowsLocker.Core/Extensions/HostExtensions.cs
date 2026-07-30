using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Core.Extensions;

public static class HostExtensions
{
    public static IHostApplicationBuilder AddWindowsLocker(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IWorkerService, TimedWorkerService>();
        builder.Services.AddScoped<IDateService, DateService>();
        builder.Services.AddScoped<ITimeService, TimeService>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder.Services.AddScoped<ILockService, WindowsLockService>();
        }
        else
        {
            builder.Services.AddScoped<ILockService, MockLockService>();
        }
        builder.Logging.ClearProviders();
        builder.Logging.AddFileLogger(options =>
        {
            options.FilePath = AppContext.BaseDirectory;
            options.LogLevel = builder.Configuration["Logging:LogLevel:Default"] ?? "Information";
        });
        return builder;
    }

    public static IHostApplicationBuilder AddWindowsLockerConsole(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IWorkerService, TimedWorkerService>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder.Services.AddScoped<ILockService, WindowsLockService>();
        }
        else
        {
            builder.Services.AddScoped<ILockService, MockLockService>();
        }
        builder.Services.AddScoped<IDateService, DateService>();
        builder.Services.AddScoped<ITimeService, TimeService>();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsoleLogger(options =>
        {
            options.LogLevel = builder.Configuration["Logging:LogLevel:Default"] ?? "Information";
        });
        return builder;
    }
}