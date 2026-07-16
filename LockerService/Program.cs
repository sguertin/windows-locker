using LockerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Library Utility Service";
});
var host = builder.Build();
host.Run();