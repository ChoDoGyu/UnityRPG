using System.Collections.Generic;

namespace UnityRPG.Quest
{
    public sealed class RuntimeQuest
    {
        private readonly List<RuntimeQuestObjective> objectives = new();

        public QuestDefinition Definition { get; }
        public IReadOnlyList<RuntimeQuestObjective> Objectives => objectives;
        public QuestState State { get; private set; } = QuestState.Active;

        public RuntimeQuest(QuestDefinition definition)
        {
            Definition = definition;

            for (int i = 0; i < definition.Objectives.Count; i++)
                objectives.Add(new RuntimeQuestObjective(definition.Objectives[i]));
        }

        public bool AddProgress(QuestObjectiveType type, string targetId, int amount)
        {
            if (State != QuestState.Active)
                return false;

            bool changed = false;

            for (int i = 0; i < objectives.Count; i++)
            {
                RuntimeQuestObjective objective = objectives[i];

                if (objective.Matches(type, targetId) && objective.AddProgress(amount))
                    changed = true;
            }

            if (changed && AreAllObjectivesCompleted())
                State = QuestState.ReadyToTurnIn;

            return changed;
        }

        public bool Complete()
        {
            if (State != QuestState.ReadyToTurnIn)
                return false;

            State = QuestState.Completed;
            return true;
        }

        private bool AreAllObjectivesCompleted()
        {
            if (objectives.Count == 0)
                return false;

            for (int i = 0; i < objectives.Count; i++)
            {
                if (!objectives[i].IsCompleted)
                    return false;
            }

            return true;
        }
    }
}