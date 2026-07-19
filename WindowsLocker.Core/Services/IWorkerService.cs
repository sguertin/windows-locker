namespace WindowsLocker.Core.Services;

public interface IWorkerService
{
    bool DoWork();

    DateTime ConvertTimeValue(string timeValue);
}
