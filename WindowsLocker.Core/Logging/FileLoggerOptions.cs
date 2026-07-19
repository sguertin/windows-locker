namespace WindowsLocker.Core.Logging;

public class FileLoggerOptions
{
    public required string FilePath { get; set; }
    
    public required string LogLevel { get; set; }
}
