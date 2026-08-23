using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Item
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "UnityRPG/Item/Item Database")]
    public sealed class ItemDatabase : ScriptableObject
    {
        [SerializeField] private ItemDefinition[] items;

        private Dictionary<string, ItemDefinition> lookup;

        public bool TryGetItem(string itemId, out ItemDefinition item)
        {
            EnsureLookup();

            if (string.IsNullOrWhiteSpace(itemId))
            {
                item = null;
                return false;
            }

            return lookup.TryGetValue(itemId, out item);
        }

        private void EnsureLookup()
        {
            if (lookup != null)
                return;

            lookup = new Dictionary<string, ItemDefinition>();

            if (items == null)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                ItemDefinition item = items[i];

                if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
                    continue;

                if (lookup.ContainsKey(item.ItemId))
                {
                    Debug.LogError($"[Item] 중복 ItemId가 있습니다: {item.ItemId}", this);
                    continue;
                }

                lookup.Add(item.ItemId, item);
            }
        }

        private void OnValidate()
        {
            lookup = null;

            if (items == null)
                return;

            HashSet<string> ids = new();

            for (int i = 0; i < items.Length; i++)
            {
                ItemDefinition item = items[i];

                if (item == null)
                    continue;

                if (string.IsNullOrWhiteSpace(item.ItemId))
                {
                    Debug.LogWarning($"[Item] {item.name}의 ItemId가 비어 있습니다.", item);
                    continue;
                }

                if (!ids.Add(item.ItemId))
                    Debug.LogError($"[Item] 중복 ItemId가 있습니다: {item.ItemId}", this);
            }
        }
    }
}