using UnityEngine;
using UnityRPG.Character.Player;
using UnityRPG.Core;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(PlayerHealth))]
    public sealed class PlayerConsumableController : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private AudioClip consumableSfx;

        private PlayerInventory inventory;
        private PlayerHealth playerHealth;
        private bool isConfigured;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            playerHealth = GetComponent<PlayerHealth>();

            isConfigured = inventory != null && playerHealth != null;
        }

        public bool TryUse(ConsumableDefinition item)
        {
            if (!isConfigured || item == null)
                return false;

            if (!inventory.HasItem(item))
                return false;

            if (!playerHealth.CanHeal)
                return false;

            if (inventory.RemoveItem(item, 1) != 1)
                return false;

            bool healed = playerHealth.TryHeal(item.HealAmount);

            if (healed)
                AudioService.Instance?.PlaySfx(consumableSfx);

            return healed;
        }
    }
}