using UnityEngine;

namespace UnityRPG.Combat
{
    public sealed class TrainingDummyHealth :
        MonoBehaviour,
        IDamageable
    {
        [SerializeField]
        [Min(1f)]
        private float maxHealth = 100f;

        [SerializeField]
        private float currentHealth;

        public float CurrentHealth =>
            currentHealth;

        private void Awake()
        {
            currentHealth =
                maxHealth;
        }

        public void TakeDamage(
            DamageInfo damageInfo)
        {
            currentHealth =
                Mathf.Max(
                    0f,
                    currentHealth - damageInfo.Amount);
        }
    }
}