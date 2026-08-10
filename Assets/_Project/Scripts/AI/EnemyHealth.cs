using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemyHealth :
        MonoBehaviour,
        IDamageable
    {
        [Header("Definition")]
        [SerializeField]
        private EnemyDefinition definition;

        private float currentHealth;
        private bool isConfigured;

        public float MaxHealth =>
            definition != null
                ? definition.MaxHealth
                : 0f;

        public float CurrentHealth =>
            currentHealth;

        public bool IsDead =>
            isConfigured &&
            currentHealth <= 0f;

        private void Awake()
        {
            if (definition == null)
            {
                Debug.LogError(
                    "[Enemy] EnemyHealth의 Enemy Definition이 설정되지 않았습니다.",
                    this);

                return;
            }

            currentHealth =
                definition.MaxHealth;

            isConfigured = true;
        }

        public void TakeDamage(
            DamageInfo damageInfo)
        {
            if (!isConfigured ||
                IsDead)
            {
                return;
            }

            float finalDamage =
                DamageCalculator.CalculateAfterDefense(
                    damageInfo.Amount,
                    definition.Defense);

            currentHealth =
                Mathf.Max(
                    0f,
                    currentHealth -
                    finalDamage);
        }
    }
}