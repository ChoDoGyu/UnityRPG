using UnityEngine;
using UnityRPG.Character.Stats;
using UnityRPG.Combat;

namespace UnityRPG.Character.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerHealth :
        MonoBehaviour,
        IDamageable
    {
        [Header("Runtime")]
        [SerializeField]
        private float currentHealth;

        private PlayerStats playerStats;
        private bool isInitialized;

        public float MaxHealth =>
            playerStats != null
                ? playerStats.MaxHealth
                : 0f;

        public float CurrentHealth =>
            currentHealth;

        public bool IsDead =>
            isInitialized &&
            currentHealth <= 0f;

        private void Awake()
        {
            playerStats =
                GetComponent<PlayerStats>();
        }

        private void Start()
        {
            currentHealth =
                playerStats.MaxHealth;

            isInitialized = true;
        }

        public void TakeDamage(
            DamageInfo damageInfo)
        {
            if (!isInitialized)
            {
                return;
            }

            if (IsDead)
            {
                return;
            }

            if (damageInfo.Amount <= 0f)
            {
                return;
            }

            float finalDamage =
                DamageCalculator.CalculateAfterDefense(
                    damageInfo.Amount,
                    playerStats.Defense);

            currentHealth =
                Mathf.Max(
                    0f,
                    currentHealth - finalDamage);
        }
    }
}