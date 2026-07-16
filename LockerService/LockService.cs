using System.Runtime.InteropServices;

namespace LockerService;


public class LockService
{
    [DllImport("user32")]
    private static extern void LockWorkStation();

    public void Lock()
    {
        LockWorkStation();
    }
}