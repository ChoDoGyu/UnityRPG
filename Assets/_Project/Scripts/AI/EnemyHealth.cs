using System;
using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        private EnemyContext context;
        private float currentHealth;
        private bool isConfigured;

        public float MaxHealth => isConfigured ? context.Definition.MaxHealth : 0f;
        public float CurrentHealth => currentHealth;
        public bool IsDead => isConfigured && currentHealth <= 0f;

        public event Action Died;
        public event Action<GameObject> DiedBy;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();

            if (!context.IsConfigured)
            {
                return;
            }

            currentHealth = context.Definition.MaxHealth;
            isConfigured = true;
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!isConfigured || IsDead)
            {
                return;
            }

            float finalDamage = DamageCalculator.CalculateAfterDefense(
                damageInfo.Amount,
                context.Definition.Defense);

            currentHealth = Mathf.Max(0f, currentHealth - finalDamage);

            if (currentHealth <= 0f)
            {
                Died?.Invoke();
                DiedBy?.Invoke(damageInfo.Source);
            }
        }
    }
}