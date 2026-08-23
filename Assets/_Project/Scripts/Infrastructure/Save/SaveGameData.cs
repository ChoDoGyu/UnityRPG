using System;
using System.Collections.Generic;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class SaveGameData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public string savedAtUtc;
        public PlayerSaveData player = new();
        public List<InventoryItemSaveData> inventory = new();
        public List<EquipmentSaveData> equipment = new();
        public List<QuestSaveData> quests = new();
        public CheckpointSaveData checkpoint = new();
    }
}