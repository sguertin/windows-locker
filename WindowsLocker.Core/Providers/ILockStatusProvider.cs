namespace WindowsLocker.Core.Providers;

public interface ILockStatusProvider
{
    bool IsLocked();
    DateTime UnlockTime();
}