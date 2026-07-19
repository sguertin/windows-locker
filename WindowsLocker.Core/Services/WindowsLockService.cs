using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace WindowsLocker.Core.Services;


public partial class WindowsLockService(ILogger<WindowsLockService> log) : ILockService
{
    [LibraryImport("user32")]
    private static partial void LockWorkStation();

    public void Lock()
    {
        log.LogInformation("Locking...");
        LockWorkStation();
    }
}
