namespace WindowsLocker.Core.Logging;

public class ConsoleLoggerOptions
{
    public string LogLevel { get; set; } = "Information";
    
    public bool Enabled { get; set; } = true;
}
