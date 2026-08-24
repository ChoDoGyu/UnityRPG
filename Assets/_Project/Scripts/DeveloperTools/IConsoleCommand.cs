namespace UnityRPG.DeveloperTools
{
    public interface IConsoleCommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }

        ConsoleCommandResult Execute(string[] args);
    }
}