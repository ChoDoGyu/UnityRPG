using System;
using UnityEngine;
using UnityRPG.AI;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyLootDropper : MonoBehaviour
    {
        [Serializable]
        private sealed class LootEntry
        {
            [SerializeField] private ItemDefinition item;
            [SerializeField, Range(0f, 1f)] private float dropChance = 1f;
            [SerializeField, Min(1)] private int minAmount = 1;
            [SerializeField, Min(1)] private int maxAmount = 1;

            public ItemDefinition Item => item;
            public float DropChance => dropChance;
            public int MinAmount => minAmount;
            public int MaxAmount => maxAmount;

            public void Validate()
            {
                dropChance = Mathf.Clamp01(dropChance);
                minAmount = Mathf.Max(1, minAmount);
                maxAmount = Mathf.Max(minAmount, maxAmount);
            }
        }

        [Header("Pickup")]
        [SerializeField] private ItemPickup pickupPrefab;
        [SerializeField] private Vector3 dropOffset = new Vector3(0f, 0.5f, 0f);
        [SerializeField, Min(0f)] private float dropSpreadRadius = 0.5f;

        [Header("Loot")]
        [SerializeField] private LootEntry[] lootEntries = Array.Empty<LootEntry>();

        private EnemyHealth enemyHealth;
        private bool isConfigured;
        private bool hasDropped;

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();

            if (!ValidateConfiguration())
                return;

            isConfigured = true;
        }

        private void OnEnable()
        {
            if (enemyHealth != null)
                enemyHealth.Died += HandleDied;
        }

        private void OnDisable()
        {
            if (enemyHealth != null)
                enemyHealth.Died -= HandleDied;
        }

        private void HandleDied()
        {
            if (!isConfigured || hasDropped)
                return;

            hasDropped = true;

            for (int i = 0; i < lootEntries.Length; i++)
                TryDrop(lootEntries[i]);
        }

        private void TryDrop(LootEntry entry)
        {
            if (UnityEngine.Random.value > entry.DropChance)
                return;

            int amount = UnityEngine.Random.Range(entry.MinAmount, entry.MaxAmount + 1);
            Vector2 spread = UnityEngine.Random.insideUnitCircle * dropSpreadRadius;
            Vector3 position = transform.position + dropOffset + new Vector3(spread.x, 0f, spread.y);

            ItemPickup pickup = Instantiate(pickupPrefab, position, Quaternion.identity);

            if (!pickup.Initialize(entry.Item, amount))
                Destroy(pickup.gameObject);
        }

        private bool ValidateConfiguration()
        {
            if (pickupPrefab == null)
            {
                Debug.LogError("[Loot] EnemyLootDropper에 ItemPickup Prefab이 없습니다.", this);
                return false;
            }

            if (lootEntries == null || lootEntries.Length == 0)
            {
                Debug.LogError("[Loot] EnemyLootDropper에 Loot Entry가 없습니다.", this);
                return false;
            }

            bool isValid = true;

            for (int i = 0; i < lootEntries.Length; i++)
            {
                LootEntry entry = lootEntries[i];

                if (entry == null)
                {
                    Debug.LogError($"[Loot] Loot Entry {i}가 비어 있습니다.", this);
                    isValid = false;
                    continue;
                }

                if (entry.Item == null)
                {
                    Debug.LogError($"[Loot] Loot Entry {i}에 ItemDefinition이 없습니다.", this);
                    isValid = false;
                }
            }

            return isValid;
        }

        private void OnValidate()
        {
            dropSpreadRadius = Mathf.Max(0f, dropSpreadRadius);

            for (int i = 0; i < lootEntries.Length; i++)
            {
                if (lootEntries[i] != null)
                    lootEntries[i].Validate();
            }
        }
    }
}