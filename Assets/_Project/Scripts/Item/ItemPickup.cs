using UnityEngine;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class ItemPickup : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField] private ItemDefinition item;
        [SerializeField, Min(1)] private int amount = 1;

        private Collider triggerCollider;
        private bool isConfigured;

        public ItemDefinition Item => item;
        public int Amount => amount;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();

            if (!triggerCollider.isTrigger)
            {
                Debug.LogError("[Item] ItemPickup의 Collider는 Is Trigger가 활성화되어 있어야 합니다.", this);
                return;
            }

            if (item == null)
            {
                Debug.LogError("[Item] ItemPickup에 ItemDefinition이 설정되지 않았습니다.", this);
                return;
            }

            isConfigured = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isConfigured || amount <= 0)
                return;

            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

            if (inventory == null)
                return;

            int added = inventory.AddItem(item, amount);

            Debug.Log($"[Item] {item.DisplayName} {added}개 획득 / 현재 보유: {inventory.GetItemCount(item)}");

            if (added <= 0)
                return;

            amount -= added;

            if (amount <= 0)
                Destroy(gameObject);
        }

        private void OnValidate()
        {
            amount = Mathf.Max(1, amount);
        }
    }
}