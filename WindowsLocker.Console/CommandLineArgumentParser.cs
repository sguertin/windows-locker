namespace WindowsLocker.Console;

public record CommandLineArguments(string Command, string? Time = null, string LogLevel = "Information");

public static class Commands
{
    public const string LOCK = "lock";
    public const string TIMER = "timer";
    public const string WORKER = "worker";
}
public static class CommandLineArgumentParser
{
    public static CommandLineArguments Handle(string[] args)
    {
        return args.Length switch
        {
            2 => new CommandLineArguments(args[0], args[1]),
            1 => new CommandLineArguments(args[0], DateTime.Now.ToString("h:mm tt")),
            0 => throw new ArgumentOutOfRangeException(nameof(args), "Missing arguments"),
            _ => new CommandLineArguments(args[0], args[1], args[2])
        };
    }
}