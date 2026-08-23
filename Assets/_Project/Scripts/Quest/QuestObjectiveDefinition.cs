using System;
using UnityEngine;

namespace UnityRPG.Quest
{
    [Serializable]
    public struct QuestObjectiveDefinition
    {
        [SerializeField] private QuestObjectiveType type;
        [SerializeField] private string targetId;
        [SerializeField, Min(1)] private int requiredAmount;

        public QuestObjectiveType Type => type;
        public string TargetId => targetId;
        public int RequiredAmount => requiredAmount;
    }
}