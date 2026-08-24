using System.Collections.Generic;

namespace UnityRPG.DeveloperTools
{
    public sealed class CommandRegistry
    {
        private readonly Dictionary<string, IConsoleCommand> commands = new();

        public IReadOnlyCollection<IConsoleCommand> Commands => commands.Values;

        public bool Register(IConsoleCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Name))
                return false;

            string name = command.Name.ToLowerInvariant();

            if (commands.ContainsKey(name))
                return false;

            commands.Add(name, command);
            return true;
        }

        public bool TryGet(string name, out IConsoleCommand command)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                command = null;
                return false;
            }

            return commands.TryGetValue(name.ToLowerInvariant(), out command);
        }

        public ConsoleCommandResult Execute(string input)
        {
            if (!CommandParser.TryParse(input, out ParsedCommand parsed))
                return ConsoleCommandResult.Fail("명령어를 입력하세요.");

            if (!TryGet(parsed.Name, out IConsoleCommand command))
                return ConsoleCommandResult.Fail($"알 수 없는 명령어입니다: {parsed.Name}");

            return command.Execute(parsed.Args);
        }
    }
}