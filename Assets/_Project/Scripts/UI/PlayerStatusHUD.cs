using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Character.Growth;
using UnityRPG.Character.Player;
using UnityRPG.Character.Stats;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class PlayerStatusHUD : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerGrowth playerGrowth;
        [SerializeField] private PlayerStats playerStats;

        [Header("Health")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthText;

        [Header("Growth")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Slider experienceSlider;
        [SerializeField] private TMP_Text experienceText;

        private void OnEnable()
        {
            if (!HasAllReferences())
                return;

            playerHealth.HealthChanged += HandleHealthChanged;
            playerGrowth.LevelChanged += HandleLevelChanged;
            playerGrowth.ExperienceChanged += HandleExperienceChanged;
            playerStats.StatChanged += HandleStatChanged;
        }

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] PlayerStatusHUD의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            RefreshAll();
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.HealthChanged -= HandleHealthChanged;

            if (playerGrowth != null)
            {
                playerGrowth.LevelChanged -= HandleLevelChanged;
                playerGrowth.ExperienceChanged -= HandleExperienceChanged;
            }

            if (playerStats != null)
                playerStats.StatChanged -= HandleStatChanged;
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            RefreshHealth(currentHealth, maxHealth);
        }

        private void HandleLevelChanged(int previousLevel, int currentLevel)
        {
            levelText.text = $"Lv.{currentLevel}";
        }

        private void HandleExperienceChanged(int currentExperience, int requiredExperience)
        {
            RefreshExperience(currentExperience, requiredExperience);
        }

        private void HandleStatChanged(StatType statType, float value)
        {
            if (statType == StatType.MaxHealth)
                RefreshHealth(playerHealth.CurrentHealth, value);
        }

        private void RefreshAll()
        {
            RefreshHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            levelText.text = $"Lv.{playerGrowth.CurrentLevel}";
            RefreshExperience(playerGrowth.CurrentExperience, playerGrowth.RequiredExperience);
        }

        private void RefreshHealth(float currentHealth, float maxHealth)
        {
            float ratio = maxHealth > 0f ? currentHealth / maxHealth : 0f;

            healthSlider.SetValueWithoutNotify(Mathf.Clamp01(ratio));
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        }

        private void RefreshExperience(int currentExperience, int requiredExperience)
        {
            if (requiredExperience <= 0)
            {
                experienceSlider.SetValueWithoutNotify(1f);
                experienceText.text = "EXP MAX";
                return;
            }

            experienceSlider.SetValueWithoutNotify(Mathf.Clamp01((float)currentExperience / requiredExperience));
            experienceText.text = $"EXP {currentExperience} / {requiredExperience}";
        }

        private bool HasAllReferences()
        {
            return playerHealth != null &&
                   playerGrowth != null &&
                   playerStats != null &&
                   healthSlider != null &&
                   healthText != null &&
                   levelText != null &&
                   experienceSlider != null &&
                   experienceText != null;
        }
    }
}