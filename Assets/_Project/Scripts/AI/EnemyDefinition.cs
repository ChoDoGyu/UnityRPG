using UnityEngine;

namespace UnityRPG.AI
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "UnityRPG/Enemy Definition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string enemyId;
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private string displayName;

        [Header("Stats")]
        [SerializeField]
        [Min(1f)]
        private float maxHealth = 50f;

        [SerializeField]
        [Min(0f)]
        private float attack = 10f;

        [SerializeField]
        [Min(0f)]
        private float defense = 2f;

        [SerializeField]
        [Min(0f)]
        private float moveSpeed = 3.5f;

        [Header("AI")]
        [SerializeField]
        [Min(0f)]
        private float detectionRange = 12f;

        [SerializeField]
        [Min(0f)]
        private float attackRange = 1.8f;

        [SerializeField]
        [Min(0f)]
        private float attackWindup = 0.35f;

        [SerializeField]
        [Min(0f)]
        private float attackRecovery = 0.4f;

        [SerializeField]
        [Min(0.01f)]
        private float attackCooldown = 1.2f;

        [Header("Reward")]
        [SerializeField, Min(0)] private int experienceReward = 10;

        public EnemyType EnemyType => enemyType;
        public string DisplayName => displayName;
        public float MaxHealth => maxHealth;
        public float Attack => attack;
        public float Defense => defense;
        public float MoveSpeed => moveSpeed;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public float AttackWindup => attackWindup;
        public float AttackRecovery => attackRecovery;
        public string EnemyId => enemyId;
        public int ExperienceReward => experienceReward;

        private void OnValidate()
        {
            if (attackRange > detectionRange)
                Debug.LogWarning($"[Enemy] {name}: Attack Range가 Detection Range보다 큽니다.", this);

            if (string.IsNullOrWhiteSpace(enemyId))
                Debug.LogWarning($"[Enemy] {name}: EnemyId가 비어 있습니다.", this);
        }
    }
}