using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WindowsLocker.Core.Services;

public class MockWorkerService(IDateService dateService, ITimeService timeService, IConfiguration configuration, ILogger<MockWorkerService> log) : IWorkerService
{
    private DateTime _timeField = dateService.Now();
    public bool DoWork()
    {
        log.LogInformation("Doing work...");
        var now = dateService.Now();
        if (timeService.MinutesElapsed(_timeField, now) < timeService.GetTimeLimit())
        {
            return false;
        }
        log.LogInformation("Time limit exceeded");
        _timeField = dateService.Now();
        return true;
    }
}