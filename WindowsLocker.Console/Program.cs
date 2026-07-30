using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WindowsLocker.Console;
using WindowsLocker.Core.Extensions;
using static WindowsLocker.Core.Constants;

var command = string.Empty;
var time = DEFAULT_TIME_LIMIT;
var logLevel = string.Empty;
try
{
    var commandArgs = CommandLineArgumentParser.Handle(args);
    command = commandArgs.Command.ToLowerInvariant();
    time = commandArgs.TimeLimit;
    logLevel = commandArgs.LogLevel;
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine(ex.Message);
    Environment.Exit(-1);
}

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration["Logging:LogLevel:Default"] = logLevel;
builder.AddWindowsLockerConsole();

var configuration = ConfigFactory.CreateConfiguration(time.ToString());
builder.Configuration.AddConfiguration(configuration);
builder.Services.AddScoped<ICommandHandler, CommandHandler>();

var host = builder.Build();
var log = host.Services.GetService(typeof(ILogger<Program>)) as ILogger<Program>;
var commandHandler = host.Services.GetService(typeof(ICommandHandler)) as ICommandHandler;
if (log!.IsEnabled(LogLevel.Information))
{
    log.LogInformation("COMMAND: '{Command}'", command.ToLowerInvariant());
    log.LogInformation("TIME: '{Time}'", time);
}

try
{
    commandHandler!.RunCommand(command, time.ToString());
    Environment.Exit(0);
}
catch (Exception ex)
{
    log.LogError(ex, "Unhandled exception: {Message}\n{StackTrace}", ex.Message, ex.StackTrace);
    Environment.Exit(-1);
}
