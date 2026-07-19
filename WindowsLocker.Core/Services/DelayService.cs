namespace WindowsLocker.Core.Services;

public class DelayService : IDelayService
{
    public async Task Delay(int milliseconds, CancellationToken stoppingToken)
    {
        await Task.Delay(milliseconds, stoppingToken);
    }
}