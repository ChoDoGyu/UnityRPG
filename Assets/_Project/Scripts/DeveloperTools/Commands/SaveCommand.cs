using UnityRPG.Infrastructure.Save;

namespace UnityRPG.DeveloperTools
{
    public sealed class SaveCommand : IConsoleCommand
    {
        private readonly SaveGameController saveGameController;

        public string Name => "save";
        public string Description => "현재 게임 상태를 저장합니다.";
        public string Usage => "save";

        public SaveCommand(SaveGameController saveGameController)
        {
            this.saveGameController = saveGameController;
        }

        public ConsoleCommandResult Execute(string[] args)
        {
            if (args.Length != 0)
                return ConsoleCommandResult.Fail($"사용법: {Usage}");

            SaveLoadStatus status = saveGameController.SaveGame();

            if (status != SaveLoadStatus.Success)
                return ConsoleCommandResult.Fail($"게임 저장 실패: {status}");

            return ConsoleCommandResult.Succeed("게임 저장 완료");
        }
    }
}