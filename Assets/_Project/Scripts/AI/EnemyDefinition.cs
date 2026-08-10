using UnityEngine;

namespace UnityRPG.AI
{
    [CreateAssetMenu(
        fileName = "EnemyDefinition",
        menuName = "UnityRPG/Enemies/Enemy Definition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private EnemyType enemyType;

        [SerializeField]
        private string displayName;

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
        private float attackRange = 2f;

        [SerializeField]
        [Min(0.01f)]
        private float attackCooldown = 1.2f;

        public EnemyType EnemyType =>
            enemyType;

        public string DisplayName =>
            displayName;

        public float MaxHealth =>
            maxHealth;

        public float Attack =>
            attack;

        public float Defense =>
            defense;

        public float MoveSpeed =>
            moveSpeed;

        public float DetectionRange =>
            detectionRange;

        public float AttackRange =>
            attackRange;

        public float AttackCooldown =>
            attackCooldown;
    }
}