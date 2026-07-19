using WindowsLocker.Service;
using WindowsLocker.Service.Extensions;
using WindowsLocker.Service.Services;
using static WindowsLocker.Service.Constants;

using LockService = WindowsLocker.Service.Services.LockService;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddFileLogger(options =>
{
    options.FilePath = AppContext.BaseDirectory;
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<ILockService, LockService>();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Library Utility Service";
});
var host = builder.Build();
host.Run();