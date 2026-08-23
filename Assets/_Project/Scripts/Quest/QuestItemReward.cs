using System;
using UnityEngine;
using UnityRPG.Item;

namespace UnityRPG.Quest
{
    [Serializable]
    public struct QuestItemReward
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField, Min(1)] private int amount;

        public ItemDefinition Item => item;
        public int Amount => amount;
    }
}