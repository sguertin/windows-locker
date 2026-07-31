using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static WindowsLocker.Core.Constants;

namespace WindowsLocker.Core.Services;

public class TimeService(IConfiguration configuration, IDateService dateService, ILogger<TimeService> log) : ITimeService
{
    public DateTime ConvertTimeValue(string timeValue)
    {
        var now = dateService.Now();
        try
        {
            var timeParts = timeValue.Split(':');
            if (timeParts.Length != 2)
            {
                throw new FormatException();
            }

            var hour = int.Parse(timeParts[0]);
            timeParts = timeParts[1].Split(' ');
            if (timeParts.Length != 2)
            {
                throw new FormatException();
            }

            var minute = int.Parse(timeParts[0]);
            var meridian = timeParts[1];
            if (meridian.Equals("PM", StringComparison.CurrentCultureIgnoreCase) && hour != 12)
            {
                hour += 12;
            }
            else if (meridian.Equals("AM", StringComparison.CurrentCultureIgnoreCase) && hour == 12)
            {
                hour = 0;
            }

            return new DateTime(now.Year, now.Month, now.Day, hour, minute, DEFAULT_SECOND);
        }
        catch (FormatException ex)
        {
            log.LogWarning(
                "The time provided: \"{TimeValue}\", is not a valid time value. Should take the form of hh:mm AM/PM e.g. 5:30 PM, 12:00 PM, 10:45 AM.",
                timeValue);
            log.LogError(ex, "{Message}", ex.Message);
            return new DateTime(now.Year, now.Month, now.Day, DEFAULT_HOUR, DEFAULT_MINUTE, DEFAULT_SECOND);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "An error occurred: {Message}", ex.Message);
            throw;
        }
    }

    public int MinutesElapsed(DateTime start, DateTime end)
    {
        return (int)(end - start).TotalMinutes;
    }

    public int GetTimeLimit()
    {
        return int.TryParse(configuration["TimeLimit"] ?? DEFAULT_TIME_LIMIT.ToString(), out var timeLimit)
            ? timeLimit
            : DEFAULT_TIME_LIMIT;
    }
}
