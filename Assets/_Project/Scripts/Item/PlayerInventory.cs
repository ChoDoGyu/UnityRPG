using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    public sealed class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory")]
        [SerializeField, Min(1)] private int maxSlots = 24;

        private readonly List<InventorySlot> slots = new List<InventorySlot>();

        public event Action Changed;

        public IReadOnlyList<InventorySlot> Slots => slots;
        public int MaxSlots => maxSlots;

        public int AddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0)
                return 0;

            int remaining = amount;

            if (item.MaxStack > 1)
                remaining = FillExistingStacks(item, remaining);

            while (remaining > 0 && slots.Count < maxSlots)
            {
                int stackCount = Mathf.Min(item.MaxStack, remaining);

                slots.Add(new InventorySlot(item, stackCount));
                remaining -= stackCount;
            }

            int added = amount - remaining;

            if (added > 0)
                Changed?.Invoke();

            return added;
        }

        public int RemoveItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0)
                return 0;

            int remaining = amount;

            for (int i = slots.Count - 1; i >= 0 && remaining > 0; i--)
            {
                InventorySlot slot = slots[i];

                if (slot.Item != item)
                    continue;

                int removed = slot.Remove(remaining);
                remaining -= removed;

                if (slot.Count <= 0)
                    slots.RemoveAt(i);
            }

            int removedTotal = amount - remaining;

            if (removedTotal > 0)
                Changed?.Invoke();

            return removedTotal;
        }

        public int GetItemCount(ItemDefinition item)
        {
            if (item == null)
                return 0;

            int total = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].Item == item)
                    total += slots[i].Count;
            }

            return total;
        }

        public bool HasItem(ItemDefinition item, int amount = 1)
        {
            return amount > 0 && GetItemCount(item) >= amount;
        }

        public void Clear()
        {
            if (slots.Count == 0)
                return;

            slots.Clear();
            Changed?.Invoke();
        }

        private int FillExistingStacks(ItemDefinition item, int amount)
        {
            int remaining = amount;

            for (int i = 0; i < slots.Count && remaining > 0; i++)
            {
                InventorySlot slot = slots[i];

                if (slot.Item != item || slot.IsFull)
                    continue;

                int added = slot.Add(remaining);
                remaining -= added;
            }

            return remaining;
        }

        private void OnValidate()
        {
            maxSlots = Mathf.Max(1, maxSlots);
        }
    }
}