using UnityEngine;

namespace UnityRPG.Character.Stats
{
    [CreateAssetMenu(
        fileName = "PlayerStatDefinition",
        menuName = "UnityRPG/Stats/Player Stat Definition")]
    public sealed class PlayerStatDefinition :
        ScriptableObject
    {
        [Header("Survival")]
        [SerializeField]
        [Min(1f)]
        private float maxHealth = 100f;

        [SerializeField]
        [Min(0f)]
        private float defense = 5f;

        [Header("Combat")]
        [SerializeField]
        [Min(0f)]
        private float attack = 10f;

        [SerializeField]
        [Range(0f, 1f)]
        private float critChance = 0.05f;

        [SerializeField]
        [Min(1f)]
        private float critDamage = 1.5f;

        [Header("Movement")]
        [SerializeField]
        [Min(0f)]
        private float moveSpeed = 5f;

        public float MaxHealth => maxHealth;

        public float Attack => attack;

        public float Defense => defense;

        public float CritChance => critChance;

        public float CritDamage => critDamage;

        public float MoveSpeed => moveSpeed;
    }
}