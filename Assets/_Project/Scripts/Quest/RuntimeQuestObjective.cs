using UnityEngine;

namespace UnityRPG.Quest
{
    public sealed class RuntimeQuestObjective
    {
        public QuestObjectiveDefinition Definition { get; }
        public int CurrentAmount { get; private set; }
        public bool IsCompleted => CurrentAmount >= Definition.RequiredAmount;

        public RuntimeQuestObjective(QuestObjectiveDefinition definition)
        {
            Definition = definition;
        }

        public bool AddProgress(int amount)
        {
            if (amount <= 0 || IsCompleted)
                return false;

            int previousAmount = CurrentAmount;
            CurrentAmount = Mathf.Min(CurrentAmount + amount, Definition.RequiredAmount);
            return CurrentAmount != previousAmount;
        }

        public bool Matches(QuestObjectiveType type, string targetId)
        {
            return Definition.Type == type && Definition.TargetId == targetId;
        }

        internal bool TryRestoreProgress(int amount)
        {
            if (amount < 0 || amount > Definition.RequiredAmount)
                return false;

            CurrentAmount = amount;
            return true;
        }
    }
}