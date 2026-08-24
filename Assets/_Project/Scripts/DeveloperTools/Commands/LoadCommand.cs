using UnityRPG.Infrastructure.Save;

namespace UnityRPG.DeveloperTools
{
    public sealed class LoadCommand : IConsoleCommand
    {
        private readonly SaveGameController saveGameController;

        public string Name => "load";
        public string Description => "저장된 게임 상태를 불러옵니다.";
        public string Usage => "load";

        public LoadCommand(SaveGameController saveGameController)
        {
            this.saveGameController = saveGameController;
        }

        public ConsoleCommandResult Execute(string[] args)
        {
            if (args.Length != 0)
                return ConsoleCommandResult.Fail($"사용법: {Usage}");

            SaveLoadStatus status = saveGameController.LoadGame();

            if (status != SaveLoadStatus.Success)
                return ConsoleCommandResult.Fail($"게임 불러오기 실패: {status}");

            return ConsoleCommandResult.Succeed("게임 불러오기 완료");
        }
    }
}