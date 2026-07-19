using WindowsLocker.Service.Services;
using static WindowsLocker.Service.Constants;
namespace WindowsLocker.Service;

public class Worker(IWorkerService workerService, ILogger log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await workerService.DoWorkAsync(stoppingToken);
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
