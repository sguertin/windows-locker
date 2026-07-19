namespace WindowsLocker.Console;

public interface ICommandHandler
{
    void RunCommand(string command, string time);
}