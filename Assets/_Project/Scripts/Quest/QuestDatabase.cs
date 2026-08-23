using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Quest
{
    [CreateAssetMenu(fileName = "QuestDatabase", menuName = "UnityRPG/Quest/Quest Database")]
    public sealed class QuestDatabase : ScriptableObject
    {
        [SerializeField] private QuestDefinition[] quests;

        private Dictionary<string, QuestDefinition> lookup;

        public bool TryGetQuest(string questId, out QuestDefinition quest)
        {
            EnsureLookup();

            if (string.IsNullOrWhiteSpace(questId))
            {
                quest = null;
                return false;
            }

            return lookup.TryGetValue(questId, out quest);
        }

        private void EnsureLookup()
        {
            if (lookup != null)
                return;

            lookup = new Dictionary<string, QuestDefinition>();

            if (quests == null)
                return;

            for (int i = 0; i < quests.Length; i++)
            {
                QuestDefinition quest = quests[i];

                if (quest == null || string.IsNullOrWhiteSpace(quest.QuestId))
                    continue;

                if (lookup.ContainsKey(quest.QuestId))
                {
                    Debug.LogError($"[Quest] 중복 QuestId가 있습니다: {quest.QuestId}", this);
                    continue;
                }

                lookup.Add(quest.QuestId, quest);
            }
        }

        private void OnValidate()
        {
            lookup = null;

            if (quests == null)
                return;

            HashSet<string> ids = new();

            for (int i = 0; i < quests.Length; i++)
            {
                QuestDefinition quest = quests[i];

                if (quest == null)
                    continue;

                if (string.IsNullOrWhiteSpace(quest.QuestId))
                {
                    Debug.LogWarning($"[Quest] {quest.name}의 QuestId가 비어 있습니다.", quest);
                    continue;
                }

                if (!ids.Add(quest.QuestId))
                    Debug.LogError($"[Quest] 중복 QuestId가 있습니다: {quest.QuestId}", this);
            }
        }
    }
}