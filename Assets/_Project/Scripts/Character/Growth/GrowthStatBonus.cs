using System;
using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.Character.Growth
{
    [Serializable]
    public struct GrowthStatBonus
    {
        [SerializeField] private StatType statType;
        [SerializeField] private float valuePerLevel;

        public StatType StatType => statType;
        public float ValuePerLevel => valuePerLevel;
    }
}