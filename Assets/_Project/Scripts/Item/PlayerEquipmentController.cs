using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(PlayerEquipment))]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerEquipmentController : MonoBehaviour
    {
        private PlayerInventory inventory;
        private PlayerEquipment equipment;
        private PlayerStats playerStats;
        private bool isConfigured;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            equipment = GetComponent<PlayerEquipment>();
            playerStats = GetComponent<PlayerStats>();

            isConfigured = inventory != null && equipment != null;
        }

        public bool TryEquip(EquipmentDefinition item)
        {
            if (!isConfigured || item == null)
                return false;

            if (equipment.IsEquipped(item))
                return false;

            if (!inventory.HasItem(item))
                return false;

            EquipmentDefinition previous = equipment.GetEquipped(item.Slot);

            if (inventory.RemoveItem(item, 1) != 1)
                return false;

            if (previous != null && inventory.AddItem(previous, 1) != 1)
            {
                inventory.AddItem(item, 1);
                return false;
            }

            if (previous != null)
                playerStats.RemoveModifiersFromSource(previous);

            ApplyStatBonuses(item);
            equipment.SetEquipped(item);

            return true;
        }

        public bool TryUnequip(EquipmentSlot slot)
        {
            if (!isConfigured)
                return false;

            EquipmentDefinition equippedItem = equipment.GetEquipped(slot);

            if (equippedItem == null)
                return false;

            if (inventory.AddItem(equippedItem, 1) != 1)
                return false;

            playerStats.RemoveModifiersFromSource(equippedItem);
            equipment.ClearSlot(slot);

            return true;
        }

        private void ApplyStatBonuses(EquipmentDefinition item)
        {
            for (int i = 0; i < item.StatBonuses.Count; i++)
            {
                EquipmentStatBonus bonus = item.StatBonuses[i];

                playerStats.AddModifier(bonus.StatType, new StatModifier(bonus.Value, item));
            }
        }
    }
}