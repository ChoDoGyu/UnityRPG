using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Quest
{
    [CreateAssetMenu(fileName = "QuestDefinition", menuName = "UnityRPG/Quest/Quest Definition")]
    public sealed class QuestDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string questId;
        [SerializeField] private string displayName;
        [TextArea][SerializeField] private string description;

        [Header("Objectives")]
        [SerializeField] private QuestObjectiveDefinition[] objectives = Array.Empty<QuestObjectiveDefinition>();

        [Header("Rewards")]
        [SerializeField, Min(0)] private int experienceReward;
        [SerializeField] private QuestItemReward[] itemRewards = Array.Empty<QuestItemReward>();

        [Header("Dialogue")]
        [TextArea][SerializeField] private string acceptDialogue;
        [TextArea][SerializeField] private string activeDialogue;
        [TextArea][SerializeField] private string readyToTurnInDialogue;
        [TextArea][SerializeField] private string completedDialogue;
        [TextArea][SerializeField] private string inventoryFullDialogue;

        public string QuestId => questId;
        public string DisplayName => displayName;
        public string Description => description;
        public IReadOnlyList<QuestObjectiveDefinition> Objectives => objectives;
        public int ExperienceReward => experienceReward;
        public IReadOnlyList<QuestItemReward> ItemRewards => itemRewards;
        public string AcceptDialogue => acceptDialogue;
        public string ActiveDialogue => activeDialogue;
        public string ReadyToTurnInDialogue => readyToTurnInDialogue;
        public string CompletedDialogue => completedDialogue;
        public string InventoryFullDialogue => inventoryFullDialogue;

        private void OnValidate()
        {
            experienceReward = Mathf.Max(0, experienceReward);

            if (string.IsNullOrWhiteSpace(questId))
                Debug.LogWarning($"[Quest] {name}의 QuestId가 비어 있습니다.", this);

            if (string.IsNullOrWhiteSpace(displayName))
                Debug.LogWarning($"[Quest] {name}의 DisplayName이 비어 있습니다.", this);
        }
    }
}