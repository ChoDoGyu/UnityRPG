using System;
using UnityEngine;
using UnityRPG.Character.Stats;
using UnityRPG.Combat;

namespace UnityRPG.Character.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(PlayerDodger))]
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Runtime")]
        [SerializeField] private float currentHealth;

        private PlayerStats playerStats;
        private PlayerDodger playerDodger;
        private bool isInitialized;

        public event Action Died;

        public float MaxHealth => playerStats != null ? playerStats.MaxHealth : 0f;
        public float CurrentHealth => currentHealth;
        public bool IsDead => isInitialized && currentHealth <= 0f;
        public bool CanHeal => isInitialized && !IsDead && currentHealth < MaxHealth;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            playerDodger = GetComponent<PlayerDodger>();
        }

        private void Start()
        {
            if (!playerStats.IsConfigured)
                return;

            currentHealth = playerStats.MaxHealth;
            isInitialized = true;
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!isInitialized || IsDead)
                return;

            if (playerDodger.IsInvulnerable)
                return;

            if (damageInfo.Amount <= 0f)
                return;

            float finalDamage = DamageCalculator.CalculateAfterDefense(damageInfo.Amount, playerStats.Defense);
            currentHealth = Mathf.Max(0f, currentHealth - finalDamage);

            if (currentHealth <= 0f)
                Died?.Invoke();
        }

        public bool TryHeal(float amount)
        {
            if (!CanHeal || amount <= 0f)
                return false;

            currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
            return true;
        }

        public bool Revive()
        {
            if (!isInitialized || !IsDead)
                return false;

            currentHealth = MaxHealth;
            return true;
        }

        public void ClampToMaxHealth()
        {
            if (!isInitialized)
                return;

            currentHealth = Mathf.Min(currentHealth, MaxHealth);
        }
    }
}