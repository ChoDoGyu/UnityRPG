using System;
using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.Item
{
    [Serializable]
    public struct EquipmentStatBonus
    {
        [SerializeField] private StatType statType;
        [SerializeField] private float value;

        public StatType StatType => statType;
        public float Value => value;
    }
}