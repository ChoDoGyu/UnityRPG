using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Character.Stats
{
    [DisallowMultipleComponent]
    public sealed class PlayerStats : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField]
        private PlayerStatDefinition definition;

        private readonly Dictionary<StatType, RuntimeStat> stats =
            new Dictionary<StatType, RuntimeStat>();

        private bool isConfigured;

        public float MaxHealth =>
            GetValue(StatType.MaxHealth);

        public float Attack =>
            GetValue(StatType.Attack);

        public float Defense =>
            GetValue(StatType.Defense);

        public float CritChance =>
            GetValue(StatType.CritChance);

        public float CritDamage =>
            GetValue(StatType.CritDamage);

        public float MoveSpeed =>
            GetValue(StatType.MoveSpeed);

        private void Awake()
        {
            if (definition == null)
            {
                Debug.LogError(
                    "[Stats] PlayerStatDefinition이 설정되지 않았습니다.",
                    this);

                return;
            }

            InitializeStats();

            isConfigured = true;
        }

        public float GetValue(StatType statType)
        {
            if (!isConfigured)
            {
                return 0f;
            }

            if (!stats.TryGetValue(
                    statType,
                    out RuntimeStat stat))
            {
                return 0f;
            }

            return stat.Value;
        }

        public void AddModifier(
            StatType statType,
            StatModifier modifier)
        {
            if (!isConfigured)
            {
                return;
            }

            if (!stats.TryGetValue(
                    statType,
                    out RuntimeStat stat))
            {
                return;
            }

            stat.AddModifier(modifier);
        }

        public void RemoveModifiersFromSource(
            object source)
        {
            if (!isConfigured)
            {
                return;
            }

            foreach (RuntimeStat stat in stats.Values)
            {
                stat.RemoveModifiersFromSource(source);
            }
        }

        private void InitializeStats()
        {
            foreach (
                StatType statType in
                Enum.GetValues(typeof(StatType)))
            {
                stats.Add(
                    statType,
                    new RuntimeStat(
                        GetBaseValue(statType)));
            }
        }

        private float GetBaseValue(
            StatType statType)
        {
            switch (statType)
            {
                case StatType.MaxHealth:
                    return definition.MaxHealth;

                case StatType.Attack:
                    return definition.Attack;

                case StatType.Defense:
                    return definition.Defense;

                case StatType.CritChance:
                    return definition.CritChance;

                case StatType.CritDamage:
                    return definition.CritDamage;

                case StatType.MoveSpeed:
                    return definition.MoveSpeed;

                default:
                    return 0f;
            }
        }
    }
}