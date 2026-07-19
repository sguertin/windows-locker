using Microsoft.Extensions.Configuration;

namespace WindowsLocker.Tests;

public record TimeEntry(string Time, DateTime Date);
public record SecondOffsetCase(int Seconds, bool Valid);
public static class WindowsLockerTestBase
{
    public const int YEAR = 2000;
    public const int MONTH = 1;
    public const int DAY = 1;
    public static IConfiguration CreateConfiguration(string time)
    {
        var settings = new Dictionary<string, string> { { "Time", time } };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
    public static DateTime CreateTime(int hour, int minute)
    {
        return new DateTime(YEAR, MONTH, DAY, hour, minute, 0);
    }
}
