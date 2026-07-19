using WindowsLocker.Core.Services;
using static WindowsLocker.Service.Constants;
namespace WindowsLocker.Service;

public class Worker(IWorkerService workerService, ILogger<Worker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = workerService.DoWork();
                if (result) // true means it triggered
                {
                    await Task.Delay(SLEEP_TIME_OUT * 60, stoppingToken);
                } 
                else
                {
                    await Task.Delay(SLEEP_TIME_OUT, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            log.LogCritical(ex, ex.Message);
            Environment.Exit(1);
        }
    }
}
