using static WindowsLocker.Service.Constants;
namespace WindowsLocker.Service.Services;

public class WorkerService(IConfiguration configuration, ILogger<WorkerService> log, ILockService lockService) : IWorkerService
{
    private readonly string _time = configuration["Time"] ?? DEFAULT_TIME;
    private const double MINUTE_BOTTOM_THRESHOLD = 0.0;
    private const double MINUTE_UPPER_THRESHOLD = 1.0;

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var lockTime = ConvertTimeValue(_time);
        var timeSpan = now.Subtract(lockTime);
        log.LogInformation("IS {TimeSpanTotalMinutes} BETWEEN {MinuteBottomThreshold} AND {MinuteUpperThreshold}?",
            timeSpan.TotalMinutes, MINUTE_BOTTOM_THRESHOLD, MINUTE_UPPER_THRESHOLD);
        if (timeSpan.TotalMinutes is > MINUTE_BOTTOM_THRESHOLD and < MINUTE_UPPER_THRESHOLD)
        {
            lockService.Lock();
            await Task.Delay(SLEEP_TIME_OUT * 59, stoppingToken);
        }

        await Task.Delay(SLEEP_TIME_OUT, stoppingToken);
    }

    public DateTime ConvertTimeValue(string timeValue)
    {
        var now = DateTime.Now;
        try
        {
            var timeParts = timeValue.Split(':');
            var hour = int.Parse(timeParts[0]);
            timeParts = timeParts[1].Split(' ');
            var minute = int.Parse(timeParts[0]);
            var meridian = timeParts[1];
            if (meridian.Equals("PM", StringComparison.CurrentCultureIgnoreCase))
            {
                hour += 12;
            }

            return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        }
        catch (FormatException ex)
        {
            log.LogWarning(
                "The time provided: \"{TimeValue}\", is not a valid time value. Should take the form of hh:mm AM/PM e.g. 5:30 PM, 12:00 PM, 10:45 AM.",
                timeValue);
            log.LogError(ex, ex.Message);
            return new DateTime(now.Year, now.Month, now.Day, DEFAULT_HOUR, DEFAULT_MINUTE, 0);
        }
        catch (Exception ex)
        {
            log.LogCritical(ex, ex.Message);
            throw;
        }
    }
}
