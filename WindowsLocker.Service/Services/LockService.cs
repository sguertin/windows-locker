using System.Runtime.InteropServices;

namespace WindowsLocker.Service.Services;


public partial class LockService : ILockService
{
    [LibraryImport("user32")]
    private static partial void LockWorkStation();

    public void Lock()
    {
        LockWorkStation();
    }
}
