using UnityEngine;

namespace UnityRPG.DeveloperTools
{
    [DisallowMultipleComponent]
    public sealed class DeveloperConsole : MonoBehaviour
    {
        private CommandRegistry registry;
        private bool isInitialized;

        public bool IsAvailable => Application.isEditor || Debug.isDebugBuild;
        public bool IsInitialized => isInitialized;
        public CommandRegistry Registry => registry;

        private void Awake()
        {
            if (!IsAvailable)
            {
                enabled = false;
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            registry = new CommandRegistry();

            RegisterBuiltInCommands();

            isInitialized = true;
        }

        private void RegisterBuiltInCommands()
        {
            registry.Register(new HelpCommand(registry));
        }

        public ConsoleCommandResult Execute(string input)
        {
            if (!IsAvailable)
                return ConsoleCommandResult.Fail("개발자 콘솔을 사용할 수 없는 빌드입니다.");

            if (!isInitialized)
                return ConsoleCommandResult.Fail("개발자 콘솔이 초기화되지 않았습니다.");

            return registry.Execute(input);
        }

        public bool RegisterCommand(IConsoleCommand command)
        {
            if (!isInitialized)
                return false;

            return registry.Register(command);
        }
    }
}