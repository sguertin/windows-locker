using static LockerService.Constants;
namespace LockerService;

public class Worker : BackgroundService
{
    private readonly LockService _lockService = new ();
    private readonly LogService _log = new (APPLICATION_NAME);
    private readonly ConfigurationService _configService = new (ConfigFilePath);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var config = await _configService.GetConfig(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var lockTime = ConvertTimeValue(config.Time);
                var timeSpan = now.Subtract(lockTime);
                if (timeSpan.TotalMinutes is > 0.0 and < 1.0)
                {
                    _lockService.Lock();
                }

                await Task.Delay(SLEEP_TIME_OUT, stoppingToken);
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

    private DateTime ConvertTimeValue(string time)
    {
        var now = DateTime.Now;
        try
        {
            var timeParts = time.Split(':');
            var hour = int.Parse(timeParts[0]);
            timeParts = timeParts[1].Split(' ');
            var minutes = int.Parse(timeParts[0]);
            var meridian = timeParts[1];
            if (meridian.Equals("PM", StringComparison.CurrentCultureIgnoreCase))
            {
                hour += 12;
            }
            return new DateTime(now.Year, now.Month, now.Day, hour, minutes, 0);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, ex.Message);
            return new DateTime(now.Year, now.Month, now.Day, DEFAULT_HOUR, DEFAULT_MINUTE, 0);
        }
    }
}