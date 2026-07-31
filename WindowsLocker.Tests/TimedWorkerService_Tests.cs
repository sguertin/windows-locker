using Microsoft.Extensions.Logging;
using Moq;
using WindowsLocker.Core.Providers;
using WindowsLocker.Core.Services;
using static WindowsLocker.Tests.WindowsLockerTestBase;

namespace WindowsLocker.Tests;

public class TimedWorkerServiceTests
{
    private static readonly DateTime Default = new (YEAR, MONTH, DAY, 0, 0, 0);
    public record TimedWorkerTestEntry(DateTime Start, DateTime Now, int ExpectedElapsedMinutes);
    public static TheoryData<int> TimeLimits =>
    [
        5,
        10,
        15,
        30,
        60,
        90,
        120,
        175,
        240
    ];
    public static TheoryData<TimedWorkerTestEntry> TimedWorkerTestEntryData => 
    [
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 0, 0, 0), 0),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 0, 15, 0), 15),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 0, 30, 0), 30),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 0, 45, 0), 45),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 1, 0, 0), 60),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 1, 15, 0), 75),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 1, 45, 0), 105),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 2, 0, 0), 120),
        new TimedWorkerTestEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 3, 0, 0), 180),
    ];
    
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<ILockService> _lockService = new();
    private readonly Mock<ITimeService> _timeService = new();
    private readonly Mock<ILogger<TimedWorkerService>> _log = new();
    private readonly Mock<ILockStatusProvider> _lockStatusProvider = new ();
    [Theory, CombinatorialData]
    public void Test_LockServiceTriggers_WhenTimeLimit_Reached(
        [CombinatorialMemberData(nameof(TimedWorkerTestEntryData))]
        TimedWorkerTestEntry entry,
        [CombinatorialMemberData(nameof(TimeLimits))]
        int timeLimit)
    {
        // Arrange
        _dateService.Setup(d => d.Now()).Returns(entry.Now);
        _lockStatusProvider.Setup(l => l.IsLocked()).Returns(false);
        _lockStatusProvider.Setup(l => l.UnlockTime()).Returns(entry.Start);
        _timeService.Setup(t => t.MinutesElapsed(entry.Start, entry.Now)).Returns(entry.ExpectedElapsedMinutes);
        _timeService.Setup(t => t.GetTimeLimit()).Returns(timeLimit);
        _lockService.Setup(l => l.Lock()).Verifiable();
        
        var expected = entry.ExpectedElapsedMinutes >= timeLimit;
        var timedWorkerService = new TimedWorkerService(
            _dateService.Object, 
            _lockService.Object, 
            _timeService.Object, 
            _log.Object, 
            _lockStatusProvider.Object
        );
        
        // Act
        var actual =  timedWorkerService.DoWork();
        
        // Assert
        Assert.Equal(expected, actual);

        if (expected)
        {
            _lockService.Verify(s => s.Lock(), Times.Once);
        }
        else
        {
            _lockService.Verify(s => s.Lock(), Times.Never);
        }
    }

    [Fact]
    public void Test_WorkerDoesNotTrigger_WhenLocked()
    {
        // Arrange
        const int TIME_LIMIT = 60;
        const bool EXPECTED = false;
        _dateService.Setup(d => d.Now()).Returns(Default.AddMinutes(TIME_LIMIT));
        _lockStatusProvider.Setup(l => l.IsLocked()).Returns(true);
        _lockStatusProvider.Setup(l => l.UnlockTime()).Returns(Default);
        _timeService.Setup(t => t.MinutesElapsed(new DateTime(), new DateTime())).Returns(TIME_LIMIT);
        _timeService.Setup(t => t.GetTimeLimit()).Returns(60);
        _lockService.Setup(l => l.Lock()).Verifiable();

        var timedWorkerService = new TimedWorkerService(
            _dateService.Object, 
            _lockService.Object, 
            _timeService.Object, 
            _log.Object, 
            _lockStatusProvider.Object
        );
        
        // Act
        var actual = timedWorkerService.DoWork();
        
        // Assert
        Assert.Equal(EXPECTED, actual);
        
        _lockService.Verify(s => s.Lock(), Times.Never);
    }
}