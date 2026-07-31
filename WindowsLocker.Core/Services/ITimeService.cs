namespace WindowsLocker.Core.Services;

public interface ITimeService
{
    DateTime ConvertTimeValue(string timeValue);

    int MinutesElapsed(DateTime start, DateTime end);

    int GetTimeLimit();
}
