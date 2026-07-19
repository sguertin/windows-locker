namespace WindowsLocker.Core.Services;

public interface IDelayService
{
    Task Delay(int milliseconds, CancellationToken stoppingToken);
}