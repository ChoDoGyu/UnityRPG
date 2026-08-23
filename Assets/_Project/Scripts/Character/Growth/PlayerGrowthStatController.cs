using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.Character.Growth
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGrowth))]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerGrowthStatController : MonoBehaviour
    {
        private PlayerGrowth playerGrowth;
        private PlayerStats playerStats;
        private bool isConfigured;

        private void Start()
        {
            playerGrowth = GetComponent<PlayerGrowth>();
            playerStats = GetComponent<PlayerStats>();

            if (!playerGrowth.IsConfigured || !playerStats.IsConfigured)
            {
                Debug.LogError("[Growth] 성장 Stat 시스템을 초기화할 수 없습니다.", this);
                return;
            }

            isConfigured = true;
            playerGrowth.LevelChanged += HandleLevelChanged;
            RefreshStatModifiers();
        }

        private void OnDestroy()
        {
            if (playerGrowth != null)
                playerGrowth.LevelChanged -= HandleLevelChanged;
        }

        private void HandleLevelChanged(int previousLevel, int currentLevel)
        {
            RefreshStatModifiers();
        }

        private void RefreshStatModifiers()
        {
            if (!isConfigured)
                return;

            playerStats.RemoveModifiersFromSource(this);

            int gainedLevels = playerGrowth.CurrentLevel - 1;

            if (gainedLevels <= 0)
                return;

            PlayerGrowthDefinition definition = playerGrowth.Definition;

            for (int i = 0; i < definition.StatBonusesPerLevel.Count; i++)
            {
                GrowthStatBonus bonus = definition.StatBonusesPerLevel[i];
                float value = bonus.ValuePerLevel * gainedLevels;

                playerStats.AddModifier(bonus.StatType, new StatModifier(value, this));
            }
        }
    }
}