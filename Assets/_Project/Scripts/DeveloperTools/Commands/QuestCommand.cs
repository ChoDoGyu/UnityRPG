using System;
using UnityRPG.Quest;

namespace UnityRPG.DeveloperTools
{
    public sealed class QuestCommand : IConsoleCommand
    {
        private readonly PlayerQuestLog questLog;
        private readonly QuestDatabase questDatabase;

        public string Name => "quest";
        public string Description => "퀘스트 상태를 조작합니다.";
        public string Usage => "quest <accept|progress|complete>";

        public QuestCommand(PlayerQuestLog questLog, QuestDatabase questDatabase)
        {
            this.questLog = questLog;
            this.questDatabase = questDatabase;
        }

        public ConsoleCommandResult Execute(string[] args)
        {
            if (args.Length == 0)
                return ConsoleCommandResult.Fail($"사용법: {Usage}");

            switch (args[0].ToLowerInvariant())
            {
                case "accept":
                    return Accept(args);

                case "progress":
                    return Progress(args);

                case "complete":
                    return Complete(args);

                default:
                    return ConsoleCommandResult.Fail($"알 수 없는 quest 명령입니다: {args[0]}");
            }
        }

        private ConsoleCommandResult Accept(string[] args)
        {
            if (args.Length != 2)
                return ConsoleCommandResult.Fail("사용법: quest accept <questId>");

            if (!questDatabase.TryGetQuest(args[1], out QuestDefinition quest))
                return ConsoleCommandResult.Fail($"퀘스트를 찾을 수 없습니다: {args[1]}");

            if (!questLog.TryAcceptQuest(quest))
                return ConsoleCommandResult.Fail($"퀘스트를 수락할 수 없습니다: {args[1]}");

            return ConsoleCommandResult.Succeed($"퀘스트 수락: {args[1]}");
        }

        private ConsoleCommandResult Progress(string[] args)
        {
            if (args.Length != 4)
                return ConsoleCommandResult.Fail("사용법: quest progress <objectiveType> <targetId> <amount>");

            if (!Enum.TryParse(args[1], true, out QuestObjectiveType objectiveType))
                return ConsoleCommandResult.Fail($"알 수 없는 ObjectiveType입니다: {args[1]}");

            if (!int.TryParse(args[3], out int amount) || amount <= 0)
                return ConsoleCommandResult.Fail("진행량은 1 이상의 정수여야 합니다.");

            if (!questLog.AddProgress(objectiveType, args[2], amount))
                return ConsoleCommandResult.Fail("진행 가능한 퀘스트 목표가 없습니다.");

            return ConsoleCommandResult.Succeed($"{objectiveType} {args[2]} +{amount}");
        }

        private ConsoleCommandResult Complete(string[] args)
        {
            if (args.Length != 2)
                return ConsoleCommandResult.Fail("사용법: quest complete <questId>");

            RuntimeQuest quest = questLog.FindQuest(args[1]);

            if (quest == null)
                return ConsoleCommandResult.Fail($"수락하지 않은 퀘스트입니다: {args[1]}");

            if (!questLog.TryCompleteQuest(args[1]))
                return ConsoleCommandResult.Fail($"퀘스트를 완료할 수 없습니다. 현재 상태: {quest.State}");

            return ConsoleCommandResult.Succeed($"퀘스트 완료: {args[1]}");
        }
    }
}