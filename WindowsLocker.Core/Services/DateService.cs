namespace WindowsLocker.Core.Services;

public class DateService : IDateService
{
    public DateTime Now()
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
    }
}