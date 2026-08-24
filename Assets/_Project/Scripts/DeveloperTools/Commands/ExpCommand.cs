using UnityRPG.Character.Growth;

namespace UnityRPG.DeveloperTools
{
    public sealed class ExpCommand : IConsoleCommand
    {
        private readonly PlayerGrowth playerGrowth;

        public string Name => "exp";
        public string Description => "플레이어에게 경험치를 지급합니다.";
        public string Usage => "exp <amount>";

        public ExpCommand(PlayerGrowth playerGrowth)
        {
            this.playerGrowth = playerGrowth;
        }

        public ConsoleCommandResult Execute(string[] args)
        {
            if (args.Length != 1)
                return ConsoleCommandResult.Fail($"사용법: {Usage}");

            if (!int.TryParse(args[0], out int amount) || amount <= 0)
                return ConsoleCommandResult.Fail("경험치는 1 이상의 정수여야 합니다.");

            int previousLevel = playerGrowth.CurrentLevel;
            int previousExperience = playerGrowth.CurrentExperience;

            playerGrowth.AddExperience(amount);

            return ConsoleCommandResult.Succeed(
                $"EXP +{amount} / Lv.{previousLevel} {previousExperience} EXP → Lv.{playerGrowth.CurrentLevel} {playerGrowth.CurrentExperience} EXP");
        }
    }
}