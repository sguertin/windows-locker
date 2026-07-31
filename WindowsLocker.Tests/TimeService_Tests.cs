using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WindowsLocker.Core.Services;
using static WindowsLocker.Tests.WindowsLockerTestBase;
namespace WindowsLocker.Tests;

public class TimeServiceTests
{
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<ILogger<TimeService>> _logger = new();
    private readonly Mock<IConfiguration> _mockConfiguration = new();
    
    public record TimeElapsedEntry(DateTime Start, DateTime End, int Expected);

    public record GetTimeLimitEntry(string TimeLimit, int Expected);
    
    public static TheoryData<TimeEntry> TimeSampleData =>
    [
        new TimeEntry("12:00 AM", new DateTime(YEAR, MONTH, DAY, 0, 0, 0)),
        new TimeEntry("8:00 am", new DateTime(YEAR, MONTH, DAY, 8, 0, 0)),
        new TimeEntry("11:35 Am", new DateTime(YEAR, MONTH, DAY, 11, 35, 0)),
        new TimeEntry("12:00 PM", new DateTime(YEAR, MONTH, DAY, 12, 0, 0)),
        new TimeEntry("5:30 pm", new DateTime(YEAR, MONTH, DAY, 17, 30, 0)),
        new TimeEntry("8:45 pM", new DateTime(YEAR, MONTH, DAY, 20, 45, 0)),
        new TimeEntry("10:15 pm", new DateTime(YEAR, MONTH, DAY, 22, 15, 0))
    ];

    public static TheoryData<TimeElapsedEntry> TimeElapsedSampleData =>
    [
        new TimeElapsedEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 0, 0, 0), 0),
        new TimeElapsedEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 0, 30, 0), 30),
        new TimeElapsedEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 2, 0, 0), 120),
        new TimeElapsedEntry(new DateTime(YEAR, MONTH, DAY, 0, 0, 0), new DateTime(YEAR, MONTH, DAY, 1, 0, 0), 60),
    ];

    public static TheoryData<GetTimeLimitEntry> GetTimeLimitSampleData =>
    [
        new GetTimeLimitEntry("15", 15),
        new GetTimeLimitEntry("35", 35),
        new GetTimeLimitEntry("60", 60),
        new GetTimeLimitEntry("90", 90),
        new GetTimeLimitEntry("120", 120),
    ];
    private static readonly DateTime Default = new (YEAR, MONTH, DAY, 0, 0, 0);
    
    [Theory]
    [MemberData(nameof(TimeSampleData))]
    public void Verify_TimeConversion(TimeEntry entry)
    {
        // Arrange
        var time = entry.Time;
        var expected = entry.Date;
        _dateService.Setup(s => s.Now()).Returns(Default);
        var timeService = new TimeService(_mockConfiguration.Object, _dateService.Object, _logger.Object);
        
        // Act
        var actual = timeService.ConvertTimeValue(time);
        
        // Assert
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [MemberData(nameof(TimeElapsedSampleData))]
    public void Verify_TimeElapsed(TimeElapsedEntry entry)
    {
        // Arrange
        _dateService.Setup(s => s.Now()).Returns(Default);
        var timeService = new TimeService(_mockConfiguration.Object, _dateService.Object, _logger.Object);
        // Act
        
        var result = timeService.MinutesElapsed(entry.Start, entry.End);
        // Assert
        
        Assert.Equal(result, entry.Expected);
    }

    [Theory]
    [MemberData(nameof(GetTimeLimitSampleData))]
    public void Verify_TimeLimit(GetTimeLimitEntry entry)
    {
        //Arrange
        _dateService.Setup(s => s.Now()).Returns(Default);
        var config = CreateTimeConfiguration(entry.TimeLimit);
        var timeService = new TimeService(config, _dateService.Object, _logger.Object);
        
        //Act
        var result = timeService.GetTimeLimit();
        
        //Assert
        Assert.Equal(result, entry.Expected);
    }
}
