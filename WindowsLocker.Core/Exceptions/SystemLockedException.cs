namespace WindowsLocker.Core.Exceptions;

public class SystemLockedException : Exception
{
    public override string Message => "The system is currently locked.";
}