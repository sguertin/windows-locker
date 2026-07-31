using Microsoft.Extensions.Configuration;

namespace WindowsLocker.Tests;

public record TimeEntry(string Time, DateTime Date);

public record SecondOffsetCase(int Seconds, bool Valid);



public static class WindowsLockerTestBase
{
    public const int YEAR = 2000;
    public const int MONTH = 1;
    public const int DAY = 1;
    public static IConfiguration CreateWorkerConfiguration(string time, string timeLimit = "60")
    {
        var settings = new Dictionary<string, string>
        {
            { "Time", time },
            { "TimeLimit", timeLimit }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
    public static IConfiguration CreateTimeConfiguration(string timeLimit)
    {
        var settings = new Dictionary<string, string>
        {
            { "TimeLimit", timeLimit }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
    public static DateTime CreateTime(int hour, int minute)
    {
        return new DateTime(YEAR, MONTH, DAY, hour, minute, 0);
    }
}
