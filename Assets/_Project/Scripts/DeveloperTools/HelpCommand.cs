using System.Text;

namespace UnityRPG.DeveloperTools
{
    public sealed class HelpCommand : IConsoleCommand
    {
        private readonly CommandRegistry registry;

        public string Name => "help";
        public string Description => "사용 가능한 명령어를 표시합니다.";
        public string Usage => "help";

        public HelpCommand(CommandRegistry registry)
        {
            this.registry = registry;
        }

        public ConsoleCommandResult Execute(string[] args)
        {
            if (args.Length > 0)
                return ConsoleCommandResult.Fail($"사용법: {Usage}");

            StringBuilder builder = new();

            foreach (IConsoleCommand command in registry.Commands)
            {
                if (builder.Length > 0)
                    builder.AppendLine();

                builder.Append($"{command.Name} - {command.Description}");
            }

            return ConsoleCommandResult.Succeed(builder.ToString());
        }
    }
}