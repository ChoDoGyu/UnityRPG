using UnityEngine;
using UnityRPG.Core;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class ItemPickup : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField] private ItemDefinition item;
        [SerializeField, Min(1)] private int amount = 1;

        [Header("SFX")]
        [SerializeField] private AudioClip pickupSfx;

        private Collider triggerCollider;
        private bool isColliderConfigured;

        public ItemDefinition Item => item;
        public int Amount => amount;

        private bool IsReady => isColliderConfigured && item != null && amount > 0;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();

            if (!triggerCollider.isTrigger)
            {
                Debug.LogError("[Item] ItemPickup의 Collider는 Is Trigger가 활성화되어 있어야 합니다.", this);
                return;
            }

            isColliderConfigured = true;
        }

        private void Start()
        {
            if (isColliderConfigured && item == null)
                Debug.LogError("[Item] ItemPickup에 ItemDefinition이 설정되지 않았습니다.", this);
        }

        public bool Initialize(ItemDefinition item, int amount)
        {
            if (!isColliderConfigured || item == null || amount <= 0)
                return false;

            this.item = item;
            this.amount = amount;
            return true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsReady)
                return;

            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

            if (inventory == null)
                return;

            int added = inventory.AddItem(item, amount);

            if (added <= 0)
                return;

            AudioService.Instance?.PlaySfx(pickupSfx);

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