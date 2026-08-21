using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Item
{
    [CreateAssetMenu(fileName = "Equipment_", menuName = "UnityRPG/Item/Equipment")]
    public sealed class EquipmentDefinition : ItemDefinition
    {
        [Header("Equipment")]
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private EquipmentStatBonus[] statBonuses;

        public EquipmentSlot Slot => slot;
        public IReadOnlyList<EquipmentStatBonus> StatBonuses => statBonuses;

        public override ItemType Type => ItemType.Equipment;
        public override int MaxStack => 1;
    }
}