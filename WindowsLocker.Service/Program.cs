using WindowsLocker.Core.Extensions;
using WindowsLocker.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.AddWindowsLocker();
builder.Services.AddHostedService<Worker>();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Library Utility Service";
});
var host = builder.Build();
host.Run();
