namespace UnityRPG.Item
{
    public sealed class InventorySlot
    {
        public ItemDefinition Item { get; }
        public int Count { get; private set; }

        public bool IsFull => Count >= Item.MaxStack;

        public InventorySlot(ItemDefinition item, int count)
        {
            Item = item;
            Count = count;
        }

        public int Add(int amount)
        {
            if (amount <= 0)
                return 0;

            int addable = Item.MaxStack - Count;
            int added = UnityEngine.Mathf.Min(addable, amount);

            Count += added;
            return added;
        }

        public int Remove(int amount)
        {
            if (amount <= 0)
                return 0;

            int removed = UnityEngine.Mathf.Min(Count, amount);

            Count -= removed;
            return removed;
        }
    }
}