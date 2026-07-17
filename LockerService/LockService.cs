using System.Runtime.InteropServices;

namespace LockerService;


public partial class LockService
{
    [LibraryImport("user32")]
    private static partial void LockWorkStation();

    public void Lock()
    {
        LockWorkStation();
    }
}