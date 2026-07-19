using WindowsLocker.Service.Services;
using static WindowsLocker.Service.Constants;
namespace WindowsLocker.Service;

public class Worker(IWorkerService workerService) : BackgroundService
{
    private readonly LogService _log = new (APPLICATION_NAME);
    
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
            _log.LogFatal(ex, ex.Message);
            Environment.Exit(1);
        }
    }
}