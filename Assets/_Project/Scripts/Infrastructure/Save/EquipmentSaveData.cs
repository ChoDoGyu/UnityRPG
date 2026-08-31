using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class EquipmentSaveData
    {
        public string slot;
        public string itemId;
    }
}