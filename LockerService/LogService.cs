namespace LockerService;

public class LogService(string applicationName)
{
    private string LogFilePath => Path.Join(AppContext.BaseDirectory, $"{applicationName}-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
    
    public void LogInformation(string message) => Log(message, "INFO");
    
    public void LogWarning(string message) => Log(message, "WARN");
    
    private void LogError(string message) => Log(message,"ERROR");
    public void LogError(Exception ex, string message) => LogError(FormatExceptionMessage(ex, message));
    
    private void LogFatal(string message) => Log(message, "FATAL");
    public void LogFatal(Exception ex, string message) => LogFatal(FormatExceptionMessage(ex, message));
    
    private void Log(string message, string level = "TRACE")
    {
        File.AppendAllText(LogFilePath, $"[{DateTime.Now:u}][{applicationName}][{level}]\t{message}" + Environment.NewLine);
    }

    private static string FormatExceptionMessage(Exception ex, string message)
    {
        if (ex.Message != message)
        {
            message = message + Environment.NewLine + ex.Message;
        }

        return $"[{ex.GetType().Name}]: {message} {Environment.NewLine} {ex.StackTrace}";
    }
}