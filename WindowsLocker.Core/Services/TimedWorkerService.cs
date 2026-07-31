using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WindowsLocker.Core.Providers;

namespace WindowsLocker.Core.Services;

public partial class TimedWorkerService(
    IDateService dateService,
    ILockService lockService,
    ITimeService timeService,
    ILogger<TimedWorkerService> log,
    ILockStatusProvider lockStatusProvider)
    : IWorkerService
{
    public bool DoWork()
    {
        var now = dateService.Now();
        
        if (lockStatusProvider.IsLocked())
        {
            LogComputerIsCurrentlyLockedAsOfNow(now.ToString("hh:mm:ss"));
            return false;
        }
        var unlockTime = lockStatusProvider.UnlockTime();
        var elapsedTime = timeService.MinutesElapsed(unlockTime, now);
        var timeLimit = timeService.GetTimeLimit();
        LogElapsedTimeElapsedTimeTimeLimitTimeLimit(elapsedTime, timeLimit);
        if (elapsedTime < timeLimit)
        {
            return false;
        }
        LogTimeLimitOfTimeLimitReachedLocking(timeLimit);
        lockService.Lock();
        return true;

    }

    [LoggerMessage(LogLevel.Information, "Computer is currently locked as of {Now}.")]
    partial void LogComputerIsCurrentlyLockedAsOfNow(string now);

    [LoggerMessage(LogLevel.Debug, "Elapsed time: {ElapsedTime} >= Time Limit: {TimeLimit}")]
    partial void LogElapsedTimeElapsedTimeTimeLimitTimeLimit(int elapsedTime, int timeLimit);

    [LoggerMessage(LogLevel.Information, "Time limit of {TimeLimit} reached, locking.")]
    partial void LogTimeLimitOfTimeLimitReachedLocking(int timeLimit);
}