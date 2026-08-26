using System;
using System.Collections.Generic;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class SaveGameData
    {
        public const int CurrentVersion = 2;

        public int version = CurrentVersion;
        public string savedAtUtc;
        public PlayerSaveData player = new();
        public List<InventoryItemSaveData> inventory = new();
        public List<EquipmentSaveData> equipment = new();
        public List<QuestSaveData> quests = new();
        public List<EncounterSaveData> encounters = new();
        public CheckpointSaveData checkpoint = new();
    }
}