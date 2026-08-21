using UnityEngine;

namespace UnityRPG.Item
{
    public abstract class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;

        [Header("Description")]
        [SerializeField, TextArea] private string description;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public string Description => description;
        public abstract ItemType Type { get; }

        public abstract int MaxStack { get; }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(itemId))
                Debug.LogWarning($"[Item] {name}의 Item ID가 비어 있습니다.", this);

            if (string.IsNullOrWhiteSpace(displayName))
                Debug.LogWarning($"[Item] {name}의 Display Name이 비어 있습니다.", this);
        }
    }
}