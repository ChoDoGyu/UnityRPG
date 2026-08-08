using UnityEngine;

namespace UnityRPG.Combat
{
    public sealed class TrainingDummyHealth :
        MonoBehaviour,
        IDamageable
    {
        [Header("Health")]
        [SerializeField]
        [Min(1f)]
        private float maxHealth = 100f;

        [SerializeField]
        private float currentHealth;

        [Header("Damage Test")]
        [SerializeField]
        private int hitCount;

        [SerializeField]
        private float lastDamageAmount;

        [SerializeField]
        private GameObject lastDamageSource;

        public float CurrentHealth =>
            currentHealth;

        public bool IsDead =>
            currentHealth <= 0f;

        public int HitCount =>
            hitCount;

        private void Awake()
        {
            ResetHealth();
        }

        public void TakeDamage(
            DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            if (damageInfo.Amount <= 0f)
            {
                return;
            }

            currentHealth =
                Mathf.Max(
                    0f,
                    currentHealth - damageInfo.Amount);

            hitCount++;

            lastDamageAmount =
                damageInfo.Amount;

            lastDamageSource =
                damageInfo.Source;
        }

        [ContextMenu("Reset Health")]
        public void ResetHealth()
        {
            currentHealth =
                maxHealth;

            hitCount = 0;

            lastDamageAmount = 0f;

            lastDamageSource = null;
        }
    }
}