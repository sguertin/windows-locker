using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using static Microsoft.Win32.SystemEvents;
using static WindowsLocker.Core.Constants;

namespace WindowsLocker.Core.Services;

public partial class TimedWorkerService : IWorkerService
{
    private readonly ILockService _lockService;
    private readonly IDateService _dateService;
    private readonly ILogger<TimedWorkerService> _log;
    private readonly int _timeLimit;
    private DateTime? _unlockTime;

    public TimedWorkerService(IDateService dateService, ILockService lockService, IConfiguration configuration,
        ILogger<TimedWorkerService> log)
    {
        _log = log;
        _timeLimit = int.Parse(configuration["TimeLimit"] ?? DEFAULT_TIME_LIMIT.ToString());
        _lockService = lockService;
        _dateService = dateService;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SessionSwitch += SystemEventsOnSessionSwitch;
        }
        else
        {
            throw new PlatformNotSupportedException(
                $"This service is not supported on the platform '{Environment.OSVersion.Platform}'.");
        }
    }

    public bool DoWork()
    {
        var now = _dateService.Now();
        if (_unlockTime == null)
        {
            return false;
        }

        var sessionTime = now.Subtract(_unlockTime.Value);
        if (sessionTime.TotalMinutes >= _timeLimit)
        {
            _lockService.Lock();
            return true;
        }

        return false;
    }

    private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        var reason = e.Reason;
        switch (reason)
        {
            case SessionSwitchReason.SessionLogoff:
            case SessionSwitchReason.SessionLock:
                LogEventEventReceivedComputerLockedResettingTime(reason);
                _unlockTime = null;
                break;
            case SessionSwitchReason.SessionLogon:
            case SessionSwitchReason.SessionUnlock:
                LogEventEventReceivedComputerLockedSettingTime(reason);
                var now = _dateService.Now();
                LogComputerUnlockedAtNow(now.ToString("hh:mm:ss"));
                _unlockTime = _dateService.Now();
                break;
            case SessionSwitchReason.ConsoleConnect:
            case SessionSwitchReason.ConsoleDisconnect:
            case SessionSwitchReason.RemoteConnect:
            case SessionSwitchReason.RemoteDisconnect:
            case SessionSwitchReason.SessionRemoteControl:
            default:
                LogUnhandledSessionSwitchReason(reason);
                break;
        }
    }


    [LoggerMessage(LogLevel.Information, "Computer unlocked at {Now}.")]
    partial void LogComputerUnlockedAtNow(string now);

    [LoggerMessage(LogLevel.Debug, "Unhandled Session Switch: {Reason}")]
    partial void LogUnhandledSessionSwitchReason(SessionSwitchReason reason);

    [LoggerMessage(LogLevel.Information, "{Reason} event received, Computer locked, setting time.")]
    partial void LogEventEventReceivedComputerLockedSettingTime(SessionSwitchReason reason);

    [LoggerMessage(LogLevel.Information, "{Reason} event received, Computer locked, resetting time.")]
    partial void LogEventEventReceivedComputerLockedResettingTime(SessionSwitchReason reason);
}