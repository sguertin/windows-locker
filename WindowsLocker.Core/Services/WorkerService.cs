using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static WindowsLocker.Service.Constants;
namespace WindowsLocker.Core.Services;

public class WorkerService(
    IConfiguration configuration,
    ILogger<WorkerService>? log,
    ILockService lockService,
    IDateService dateService,
    ITimeService timeService) : IWorkerService
{
    private readonly string _time = configuration["Time"] ?? DEFAULT_TIME;
    private const double MINUTE_BOTTOM_THRESHOLD = 0.0;
    private const double MINUTE_UPPER_THRESHOLD = 1.0;

    public bool DoWork()
    {
        var now = dateService.Now();
        var lockTime = timeService.ConvertTimeValue(_time);
        var timeSpan = now.Subtract(lockTime);
        if (log!.IsEnabled(LogLevel.Information))
        {
            log.LogInformation("IS {TimeSpanTotalMinutes} BETWEEN {MinuteBottomThreshold} AND {MinuteUpperThreshold}?",
                timeSpan.TotalMinutes, MINUTE_BOTTOM_THRESHOLD, MINUTE_UPPER_THRESHOLD);
        }
        if (timeSpan.TotalMinutes is < MINUTE_BOTTOM_THRESHOLD or > MINUTE_UPPER_THRESHOLD)
        {
            return false;
        }
        lockService.Lock();
        return true;
    }
}
