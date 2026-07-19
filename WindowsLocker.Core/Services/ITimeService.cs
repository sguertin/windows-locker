namespace WindowsLocker.Core.Services;

public interface ITimeService
{
    DateTime ConvertTimeValue(string timeValue);
}