using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    public sealed class PlayerEquipment : MonoBehaviour
    {
        private readonly Dictionary<EquipmentSlot, EquipmentDefinition> equippedItems =
            new Dictionary<EquipmentSlot, EquipmentDefinition>();

        public event Action Changed;

        public IReadOnlyDictionary<EquipmentSlot, EquipmentDefinition> EquippedItems => equippedItems;

        public EquipmentDefinition GetEquipped(EquipmentSlot slot)
        {
            equippedItems.TryGetValue(slot, out EquipmentDefinition equipment);
            return equipment;
        }

        public bool IsEquipped(EquipmentDefinition equipment)
        {
            if (equipment == null)
                return false;

            return GetEquipped(equipment.Slot) == equipment;
        }

        public bool HasEquipment(EquipmentSlot slot)
        {
            return equippedItems.ContainsKey(slot);
        }

        internal void SetEquipped(EquipmentDefinition equipment)
        {
            if (equipment == null)
                return;

            equippedItems[equipment.Slot] = equipment;
            Changed?.Invoke();
        }

        internal EquipmentDefinition ClearSlot(EquipmentSlot slot)
        {
            if (!equippedItems.TryGetValue(slot, out EquipmentDefinition equipment))
                return null;

            equippedItems.Remove(slot);
            Changed?.Invoke();

            return equipment;
        }
    }
}