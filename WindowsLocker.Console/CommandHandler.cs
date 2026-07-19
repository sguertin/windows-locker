using Microsoft.Extensions.Logging;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Console;

public class CommandHandler(
    ILockService lockService,
    ITimeService timeService,
    IWorkerService workerService,
    ILogger<CommandHandler> log) : ICommandHandler
{
    public void RunCommand(string command, string time)
    {
        switch (command.ToLowerInvariant())
        {
            case Commands.LOCK:
                if (log.IsEnabled(LogLevel.Information))
                {
                    log.LogInformation("Locking system now...");
                }
                lockService!.Lock();
                break;
            case Commands.TIMER:
                var timerResult = timeService!.ConvertTimeValue(time);
                if (log.IsEnabled(LogLevel.Information))
                {
                    log.LogInformation("ConvertTimeValue: {Result:t}", timerResult);
                }
                break;
            case Commands.WORKER:
                var workerResult = workerService!.DoWork();
                if (workerResult && log.IsEnabled(LogLevel.Information))
                {
                    log.LogInformation("workerService.DoWork: TRUE");
                    log.LogInformation("Lock Triggered!");
                }
                else if (log.IsEnabled(LogLevel.Information))
                {
                    log.LogInformation("workerService.DoWork: FALSE");
                    log.LogInformation("No Lock Triggered!");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(command);
        }
    }
}
