using Microsoft.Extensions.Logging;

namespace WindowsLocker.Core.Services;

public class MockLockService(ILogger<MockLockService> log) : ILockService
{
    public void Lock()
    {
        log.LogInformation("Locking...");
    }
}