using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WindowsLocker.Core.Services;
using static WindowsLocker.Tests.WindowsLockerTestBase;
namespace WindowsLocker.Tests;

public class WorkerServiceTests
{
    private readonly Mock<ILockService> _lockService = new();
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<ITimeService> _timeService = new();
    private readonly Mock<ILogger<WorkerService>> _logger = new();

    public static TheoryData<TimeEntry> LockOnTimeData =>
    [
        new TimeEntry("12:00 AM", new DateTime(YEAR, MONTH, DAY, 0, 0, 0)),
        new TimeEntry("8:00 AM", new DateTime(YEAR, MONTH, DAY, 8, 0, 0)),
        new TimeEntry("11:35 AM", new DateTime(YEAR, MONTH, DAY, 11, 35, 0)),
        new TimeEntry("12:00 PM", new DateTime(YEAR, MONTH, DAY, 12, 0, 0)),
        new TimeEntry("5:30 PM", new DateTime(YEAR, MONTH, DAY, 17, 30, 0)),
        new TimeEntry("8:45 PM", new DateTime(YEAR, MONTH, DAY, 20, 45, 0)),
        new TimeEntry("10:15 PM", new DateTime(YEAR, MONTH, DAY, 22, 15, 0))
    ];
    
    public static TheoryData<SecondOffsetCase> SecondOffsetCases =>
    [
        new SecondOffsetCase(0, true),
        new SecondOffsetCase(15, true),
        new SecondOffsetCase(30, true),
        new SecondOffsetCase(45, true),
        new SecondOffsetCase(60, true),
        new SecondOffsetCase(90, false),
        new SecondOffsetCase(120, false)
    ];
    
    public WorkerServiceTests()
    {
        _lockService.Setup(s => s.Lock()).Verifiable();
    }
    
    [Theory, CombinatorialData]
    public void Verify_LockOnTime(
        [CombinatorialMemberData(nameof(LockOnTimeData))]
        TimeEntry input,
        [CombinatorialMemberData(nameof(SecondOffsetCases))]
        SecondOffsetCase offsets)
    {
        // Arrange
        var currentTime = input.Date.AddSeconds(offsets.Seconds);
        var config = CreateWorkerConfiguration(input.Time);
        _dateService.Setup(s => s.Now()).Returns(currentTime);
        _timeService.Setup(s => s.ConvertTimeValue(input.Time)).Returns(input.Date);
        var workerService = new WorkerService(
            config, 
            _logger.Object, 
            _lockService.Object, 
            _dateService.Object,
            _timeService.Object
        );
        
        // Act
        workerService.DoWork();

        // Assert
        if (offsets.Valid)
        {
            _lockService.Verify(s => s.Lock(), Times.Once);
        }
        else
        {
            _lockService.Verify(s => s.Lock(), Times.Never);
        }
    }
}
