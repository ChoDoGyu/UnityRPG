using System;
using UnityEngine;

namespace UnityRPG.Character.Growth
{
    [DisallowMultipleComponent]
    public sealed class PlayerGrowth : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField] private PlayerGrowthDefinition definition;

        [Header("Runtime")]
        [SerializeField, Min(1)] private int currentLevel = 1;
        [SerializeField, Min(0)] private int currentExperience;

        private bool isConfigured;

        public event Action<int, int> LevelChanged;
        public event Action<int, int> ExperienceChanged;

        public int CurrentLevel => currentLevel;
        public int CurrentExperience => currentExperience;
        public int RequiredExperience => IsMaxLevel ? 0 : definition.GetRequiredExperience(currentLevel);
        public bool IsMaxLevel => isConfigured && currentLevel >= definition.MaxLevel;
        public bool IsConfigured => isConfigured;

        private void Awake()
        {
            if (definition == null)
            {
                Debug.LogError("[Growth] PlayerGrowthDefinition이 설정되지 않았습니다.", this);
                return;
            }

            currentLevel = Mathf.Clamp(currentLevel, 1, definition.MaxLevel);

            if (currentLevel >= definition.MaxLevel)
                currentExperience = 0;
            else
                currentExperience = Mathf.Clamp(currentExperience, 0, definition.GetRequiredExperience(currentLevel) - 1);

            isConfigured = true;
        }

        public bool AddExperience(int amount)
        {
            if (!isConfigured || amount <= 0 || IsMaxLevel)
                return false;

            int previousLevel = currentLevel;
            currentExperience += amount;

            while (!IsMaxLevel)
            {
                int requiredExperience = definition.GetRequiredExperience(currentLevel);

                if (currentExperience < requiredExperience)
                    break;

                currentExperience -= requiredExperience;
                currentLevel++;
            }

            if (IsMaxLevel)
                currentExperience = 0;

            if (currentLevel != previousLevel)
                LevelChanged?.Invoke(previousLevel, currentLevel);

            ExperienceChanged?.Invoke(currentExperience, RequiredExperience);
            return true;
        }

        private void OnValidate()
        {
            currentLevel = Mathf.Max(1, currentLevel);
            currentExperience = Mathf.Max(0, currentExperience);
        }
    }
}