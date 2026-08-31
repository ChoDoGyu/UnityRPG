using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class InventoryItemSaveData
    {
        public string itemId;
        public int count;
    }
}