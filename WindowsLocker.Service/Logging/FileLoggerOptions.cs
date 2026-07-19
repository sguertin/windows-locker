using static WindowsLocker.Service.Constants;

namespace WindowsLocker.Service.Logging;

public class FileLoggerOptions
{
    public required string FilePath { get; set; }
}
