using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using WindowsLocker.Core.Exceptions;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Core.Providers;

public partial class SystemLockStatusProvider : ILockStatusProvider
{
    private readonly ILogger<SystemLockStatusProvider> _log;
    private readonly IDateService _dateService;
    private DateTime? _unlockTime;
    public SystemLockStatusProvider(ILogger<SystemLockStatusProvider> log, IDateService dateService)
    {
        _log = log;
        _dateService = dateService;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
        }
        else
        {
            throw new PlatformNotSupportedException(
                $"This service is not supported on the platform '{Environment.OSVersion.Platform}'.");
        }
    }

    public bool IsLocked()
    {
        return _unlockTime == null;
    }
    public DateTime UnlockTime()
    {
        return _unlockTime ?? throw new SystemLockedException();
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