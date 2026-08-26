using System;
using System.Collections.Generic;
using UnityEngine;
using UnityRPG.Quest;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerQuestLog))]
    public sealed class PlayerQuestSaveAdapter : MonoBehaviour
    {
        [SerializeField] private QuestDatabase questDatabase;

        private PlayerQuestLog questLog;

        private void Awake()
        {
            questLog = GetComponent<PlayerQuestLog>();
        }

        public bool Capture(SaveGameData data)
        {
            if (data == null || data.quests == null)
                return false;

            data.quests.Clear();

            for (int i = 0; i < questLog.Quests.Count; i++)
            {
                RuntimeQuest quest = questLog.Quests[i];
                QuestSaveData questData = new()
                {
                    questId = quest.Definition.QuestId,
                    state = quest.State.ToString()
                };

                for (int j = 0; j < quest.Objectives.Count; j++)
                {
                    questData.objectives.Add(new QuestObjectiveSaveData
                    {
                        objectiveIndex = j,
                        currentAmount = quest.Objectives[j].CurrentAmount
                    });
                }

                data.quests.Add(questData);
            }

            return true;
        }

        public bool Restore(SaveGameData data)
        {
            if (data == null || questDatabase == null || data.quests == null)
                return false;

            if (!TryBuildRestoredQuests(data.quests, out List<RuntimeQuest> restoredQuests))
                return false;

            questLog.ClearForRestore();

            for (int i = 0; i < restoredQuests.Count; i++)
            {
                if (!questLog.AddRestoredQuest(restoredQuests[i]))
                    return false;
            }

            questLog.NotifyRestoreCompleted();
            return true;
        }

        private bool TryBuildRestoredQuests(List<QuestSaveData> saveQuests, out List<RuntimeQuest> restoredQuests)
        {
            restoredQuests = new List<RuntimeQuest>();
            HashSet<string> usedQuestIds = new();

            for (int i = 0; i < saveQuests.Count; i++)
            {
                QuestSaveData saveQuest = saveQuests[i];

                if (saveQuest == null || string.IsNullOrWhiteSpace(saveQuest.questId) || !usedQuestIds.Add(saveQuest.questId))
                    return false;

                if (!questDatabase.TryGetQuest(saveQuest.questId, out QuestDefinition definition))
                    return false;

                if (!Enum.TryParse(saveQuest.state, out QuestState state))
                    return false;

                if (saveQuest.objectives == null || saveQuest.objectives.Count != definition.Objectives.Count)
                    return false;

                RuntimeQuest runtimeQuest = new RuntimeQuest(definition);
                HashSet<int> usedObjectiveIndices = new();

                for (int j = 0; j < saveQuest.objectives.Count; j++)
                {
                    QuestObjectiveSaveData objectiveData = saveQuest.objectives[j];

                    if (objectiveData == null || !usedObjectiveIndices.Add(objectiveData.objectiveIndex))
                        return false;

                    if (!runtimeQuest.TryRestoreObjectiveProgress(objectiveData.objectiveIndex, objectiveData.currentAmount))
                        return false;
                }

                if (!runtimeQuest.TryRestoreState(state))
                    return false;

                restoredQuests.Add(runtimeQuest);
            }

            return true;
        }
    }
}