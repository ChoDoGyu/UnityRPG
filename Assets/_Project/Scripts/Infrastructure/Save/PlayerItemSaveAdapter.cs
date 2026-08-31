using System;
using System.Collections.Generic;
using UnityEngine;
using UnityRPG.Item;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(PlayerEquipment))]
    [RequireComponent(typeof(PlayerEquipmentController))]
    public sealed class PlayerItemSaveAdapter : MonoBehaviour
    {
        [SerializeField] private ItemDatabase itemDatabase;

        private PlayerInventory inventory;
        private PlayerEquipment equipment;
        private PlayerEquipmentController equipmentController;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            equipment = GetComponent<PlayerEquipment>();
            equipmentController = GetComponent<PlayerEquipmentController>();
        }

        public bool Capture(SaveGameData data)
        {
            if (data == null || data.inventory == null || data.equipment == null)
                return false;

            data.inventory.Clear();
            data.equipment.Clear();

            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                InventorySlot slot = inventory.Slots[i];

                data.inventory.Add(new InventoryItemSaveData
                {
                    itemId = slot.Item.ItemId,
                    count = slot.Count
                });
            }

            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                EquipmentDefinition item = equipment.GetEquipped(slot);

                if (item == null)
                    continue;

                data.equipment.Add(new EquipmentSaveData
                {
                    slot = slot.ToString(),
                    itemId = item.ItemId
                });
            }

            return true;
        }

        public bool Restore(SaveGameData data)
        {
            if (data == null || itemDatabase == null)
                return false;

            if (!ValidateInventoryData(data.inventory) || !ValidateEquipmentData(data.equipment))
                return false;

            equipmentController.ClearForRestore();
            inventory.Clear();

            for (int i = 0; i < data.inventory.Count; i++)
            {
                InventoryItemSaveData saveItem = data.inventory[i];
                itemDatabase.TryGetItem(saveItem.itemId, out ItemDefinition item);

                if (inventory.AddItem(item, saveItem.count) != saveItem.count)
                    return false;
            }

            for (int i = 0; i < data.equipment.Count; i++)
            {
                EquipmentSaveData saveEquipment = data.equipment[i];
                itemDatabase.TryGetItem(saveEquipment.itemId, out ItemDefinition item);

                if (!equipmentController.TryEquipForRestore((EquipmentDefinition)item))
                    return false;
            }

            return true;
        }

        private bool ValidateInventoryData(List<InventoryItemSaveData> saveItems)
        {
            if (saveItems == null || saveItems.Count > inventory.MaxSlots)
                return false;

            for (int i = 0; i < saveItems.Count; i++)
            {
                InventoryItemSaveData saveItem = saveItems[i];

                if (saveItem == null || !itemDatabase.TryGetItem(saveItem.itemId, out ItemDefinition item))
                    return false;

                if (saveItem.count < 1 || saveItem.count > item.MaxStack)
                    return false;
            }

            return true;
        }

        private bool ValidateEquipmentData(List<EquipmentSaveData> saveEquipment)
        {
            if (saveEquipment == null)
                return false;

            HashSet<EquipmentSlot> usedSlots = new();

            for (int i = 0; i < saveEquipment.Count; i++)
            {
                EquipmentSaveData entry = saveEquipment[i];

                if (entry == null || !Enum.TryParse(entry.slot, out EquipmentSlot slot) || !usedSlots.Add(slot))
                    return false;

                if (!itemDatabase.TryGetItem(entry.itemId, out ItemDefinition item))
                    return false;

                if (item is not EquipmentDefinition equipmentItem || equipmentItem.Slot != slot)
                    return false;
            }

            return true;
        }
    }
}