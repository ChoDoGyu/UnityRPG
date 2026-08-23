using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Character.Growth
{
    [CreateAssetMenu(fileName = "PlayerGrowthDefinition", menuName = "UnityRPG/Growth/Player Growth Definition")]
    public sealed class PlayerGrowthDefinition : ScriptableObject
    {
        [Header("Level")]
        [SerializeField, Min(2)] private int maxLevel = 10;
        [SerializeField, Min(1)] private int baseExperienceToNextLevel = 100;
        [SerializeField, Min(1f)] private float experienceGrowthMultiplier = 1.35f;

        [Header("Stat Growth")]
        [SerializeField] private GrowthStatBonus[] statBonusesPerLevel = Array.Empty<GrowthStatBonus>();

        public int MaxLevel => maxLevel;
        public IReadOnlyList<GrowthStatBonus> StatBonusesPerLevel => statBonusesPerLevel;

        public int GetRequiredExperience(int level)
        {
            if (level < 1 || level >= maxLevel)
                return 0;

            return Mathf.Max(1, Mathf.RoundToInt(baseExperienceToNextLevel * Mathf.Pow(experienceGrowthMultiplier, level - 1)));
        }

        private void OnValidate()
        {
            maxLevel = Mathf.Max(2, maxLevel);
            baseExperienceToNextLevel = Mathf.Max(1, baseExperienceToNextLevel);
            experienceGrowthMultiplier = Mathf.Max(1f, experienceGrowthMultiplier);
        }
    }
}