namespace LockerService;

public static class Constants
{
    public const int SLEEP_TIME_OUT = 1000;
    public const int DEFAULT_HOUR = 17;
    public const int DEFAULT_MINUTE = 30;
    public const string APPLICATION_NAME = "LibraryUtility";
    public static readonly string ConfigFilePath =
        Path.Join(AppContext.BaseDirectory, "appSettings.json");
}