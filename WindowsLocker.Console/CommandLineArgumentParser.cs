using static WindowsLocker.Core.Constants;
namespace WindowsLocker.Console;

public record CommandLineArguments(string Command, int TimeLimit, string LogLevel = "Information");

public static class Commands
{
    public const string LOCK = "lock";
    public const string TIMER = "timer";
    public const string WORKER = "worker";
    public static readonly IList<string> ValidCommands = new List<string>{LOCK, TIMER, WORKER};
}

public static class CommandLineArgumentParser
{
    public static CommandLineArguments Handle(string[] args)
    {
        return args.Length switch
        {
            2 => new CommandLineArguments(ParseCommand(args[0]), ParseTimeLimit(args[1])),
            1 => new CommandLineArguments(ParseCommand(args[0]), DEFAULT_TIME_LIMIT),
            0 => throw new ArgumentOutOfRangeException(nameof(args), "Missing arguments"),
            _ => new CommandLineArguments(ParseCommand(args[0]), ParseTimeLimit(args[1]), args[2])
        };
    }

    public static string ParseCommand(string arg)
    {
        return Commands.ValidCommands.Contains(arg) 
            ? arg 
            : throw new ArgumentException($"Unrecognized command: {arg}");
    }
    public static int ParseTimeLimit(string arg)
    {
        return int.TryParse(arg, out var result)
            ? result
            : throw new ArgumentException($"Invalid time limit: {arg}, expected an integer.");
    }
}
