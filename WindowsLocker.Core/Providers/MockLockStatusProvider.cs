using Microsoft.Extensions.Configuration;
using WindowsLocker.Core.Exceptions;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Core.Providers;

public class MockLockStatusProvider(ITimeService timeService, IDateService dateService) : ILockStatusProvider
{
    private DateTime? _toggleTime;
    private bool _isLocked;
    private readonly int _timeLimit = timeService.GetTimeLimit();

    public bool IsLocked()
    {
        var now = dateService.Now();
        if (_toggleTime == null)
        {
            _toggleTime = now;
            _isLocked = false; // Unlocked
        }
        if (timeService.MinutesElapsed(_toggleTime.Value, now) < _timeLimit)
        {
            return _isLocked;
        }
        // If the system was locked, set the toggle time to now;
        // if the system is unlocked, set the toggle time to trigger again in 5 minutes
        _toggleTime = _isLocked ? now : _toggleTime.Value.AddMinutes(5);
        _isLocked = !_isLocked;
        return _isLocked;
    }

    public DateTime UnlockTime()
    {
        if (_isLocked)
        {
            throw new SystemLockedException();
        }
        return _toggleTime ?? throw new SystemLockedException();
    }
}