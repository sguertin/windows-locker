namespace WindowsLocker.Service.Services;

public interface IWorkerService
{
    Task DoWorkAsync(CancellationToken stoppingToken);

    DateTime ConvertTimeValue(string timeValue);
}