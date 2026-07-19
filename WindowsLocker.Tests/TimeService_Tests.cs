using Microsoft.Extensions.Logging;
using Moq;
using WindowsLocker.Core.Services;
using static WindowsLocker.Tests.WindowsLockerTestBase;
namespace WindowsLocker.Tests;

public class TimeServiceTests
{
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<ILogger<TimeService>> _logger = new();
    
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
    private static readonly DateTime Default = new (YEAR, MONTH, DAY, 0, 0, 0);
    [Theory]
    [MemberData(nameof(TimeSampleData))]
    public void Verify_TimeConversion(TimeEntry entry)
    {
        // Arrange
        var time = entry.Time;
        var expected = entry.Date;
        _dateService.Setup(s => s.Now()).Returns(Default);
        var timeService = new TimeService(_logger.Object, _dateService.Object);
        
        // Act
        var actual = timeService.ConvertTimeValue(time);
        
        // Assert
        Assert.Equal(expected, actual);
    }
}
