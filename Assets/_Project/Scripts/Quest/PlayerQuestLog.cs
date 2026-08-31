using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Quest
{
    [DisallowMultipleComponent]
    public sealed class PlayerQuestLog : MonoBehaviour
    {
        private readonly List<RuntimeQuest> quests = new();

        public event Action<RuntimeQuest> QuestAccepted;
        public event Action<RuntimeQuest> QuestProgressChanged;
        public event Action<RuntimeQuest> QuestReadyToTurnIn;
        public event Action<RuntimeQuest> QuestCompleted;
        public event Action QuestsRestored;

        public IReadOnlyList<RuntimeQuest> Quests => quests;

        public bool TryAcceptQuest(QuestDefinition definition)
        {
            if (definition == null || FindQuest(definition.QuestId) != null)
                return false;

            RuntimeQuest quest = new RuntimeQuest(definition);
            quests.Add(quest);
            QuestAccepted?.Invoke(quest);
            return true;
        }

        public bool AddProgress(QuestObjectiveType type, string targetId, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(targetId) || amount <= 0)
                return false;

            bool changed = false;

            for (int i = 0; i < quests.Count; i++)
            {
                RuntimeQuest quest = quests[i];
                QuestState previousState = quest.State;

                if (!quest.AddProgress(type, targetId, amount))
                    continue;

                changed = true;
                QuestProgressChanged?.Invoke(quest);

                if (previousState != QuestState.ReadyToTurnIn && quest.State == QuestState.ReadyToTurnIn)
                    QuestReadyToTurnIn?.Invoke(quest);
            }

            return changed;
        }

        public bool TryCompleteQuest(string questId)
        {
            RuntimeQuest quest = FindQuest(questId);

            if (quest == null || !quest.Complete())
                return false;

            QuestCompleted?.Invoke(quest);
            return true;
        }

        internal void ClearForRestore()
        {
            quests.Clear();
        }

        internal bool TryAddRestoredQuest(RuntimeQuest quest)
        {
            if (quest == null || FindQuest(quest.Definition.QuestId) != null)
                return false;

            quests.Add(quest);
            return true;
        }

        public RuntimeQuest FindQuest(string questId)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].Definition.QuestId == questId)
                    return quests[i];
            }

            return null;
        }

        internal void NotifyRestoreCompleted()
        {
            QuestsRestored?.Invoke();
        }
    }
}