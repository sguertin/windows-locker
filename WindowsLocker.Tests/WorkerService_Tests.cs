using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WindowsLocker.Core.Services;
using Xunit.v3;
namespace WindowsLocker.Tests;

public class WorkerServiceTests
{
    private readonly Mock<ILockService> _lockService = new();
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<ILogger<WorkerService>> _logger = new();
    
    private static IConfiguration CreateConfiguration(string time)
    {
        var settings = new Dictionary<string, string> { { "Time", time } };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }

    private static DateTime CreateTime(int hour, int minute)
    {
        return new DateTime(YEAR, MONTH, DAY, hour, minute, 0);
    }
    public WorkerServiceTests()
    {
        _lockService.Setup(s => s.Lock()).Verifiable();
        _dateService.Setup(s => s.Now()).Returns(new DateTime(YEAR, MONTH, DAY, 0, 0, 0));
    }

    private const int YEAR = 2000;
    private const int MONTH = 1;
    private const int DAY = 1;
    public record TimeEntry(string Time, DateTime Date);

    public static TheoryData<TimeEntry> TimeSampleData =>
    [
        new TimeEntry("12:00 AM", new DateTime(YEAR, MONTH, DAY, 0, 0, 0)),
        new TimeEntry("8:00 AM", new DateTime(YEAR, MONTH, DAY, 8, 0, 0)),
        new TimeEntry("11:35 AM", new DateTime(YEAR, MONTH, DAY, 11, 35, 0)),
        new TimeEntry("12:00 PM", new DateTime(YEAR, MONTH, DAY, 12, 0, 0)),
        new TimeEntry("5:30 PM", new DateTime(YEAR, MONTH, DAY, 17, 30, 0)),
        new TimeEntry("8:45 PM", new DateTime(YEAR, MONTH, DAY, 20, 45, 0)),
        new TimeEntry("10:15 PM", new DateTime(YEAR, MONTH, DAY, 22, 15, 0))
    ];

    public record SecondOffsetCase(int Seconds, bool Valid);

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
    
    private static Mock<ILockService> LockService() {
        var lockService = new Mock<ILockService>();
        lockService.Setup(s => s.Lock()).Verifiable();
        return lockService;
    }
    
    [Theory]
    [MemberData(nameof(TimeSampleData))]
    public void Verify_TimeConversion(TimeEntry entry)
    {
        // Arrange
        var time = entry.Time;
        var expected = entry.Date;
        var config = CreateConfiguration(time);
        var workerService = new WorkerService(config, _logger.Object, _lockService.Object, _dateService.Object);
        
        // Act
        var actual = workerService.ConvertTimeValue(time);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory, CombinatorialData]
    public void Verify_LockOnTime(
        [CombinatorialMemberData(nameof(TimeSampleData))] TimeEntry input,
        [CombinatorialMemberData(nameof(SecondOffsetCases))] SecondOffsetCase offsets)
    {
        // Arrange
        var time = input.Time;
        var currentTime = input.Date.AddSeconds(offsets.Seconds);
        var config = CreateConfiguration(time);
        var lockService = LockService();
        _dateService.Setup(s => s.Now()).Returns(currentTime);

        // Act
        var workerService = new WorkerService(config, _logger.Object, lockService.Object, _dateService.Object);
        workerService.DoWork();

        // Assert
        if (offsets.Valid)
        {
            lockService.Verify(s => s.Lock(), Times.Once);
        }
        else
        {
            lockService.Verify(s => s.Lock(), Times.Never);
        }
    }
}
