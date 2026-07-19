using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Core.Extensions;

public static class HostExtensions
{
    public static IHostApplicationBuilder AddWindowsLocker(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IWorkerService, WorkerService>();
        builder.Services.AddScoped<ILockService, LockService>();
        builder.Services.AddScoped<IDateService, DateService>();
        builder.Logging.ClearProviders();
        builder.Logging.AddFileLogger(options =>
        {
            options.FilePath = AppContext.BaseDirectory;
        });
        return builder;
    }
}