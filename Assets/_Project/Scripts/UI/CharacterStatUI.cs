using TMPro;
using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class CharacterStatUI : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerStats playerStats;

        [Header("Values")]
        [SerializeField] private TMP_Text maxHealthValueText;
        [SerializeField] private TMP_Text attackValueText;
        [SerializeField] private TMP_Text defenseValueText;
        [SerializeField] private TMP_Text critChanceValueText;
        [SerializeField] private TMP_Text critDamageValueText;
        [SerializeField] private TMP_Text moveSpeedValueText;

        private void OnEnable()
        {
            if (!HasAllReferences())
                return;

            playerStats.StatChanged += HandleStatChanged;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (playerStats != null)
                playerStats.StatChanged -= HandleStatChanged;
        }

        private void HandleStatChanged(StatType statType, float value)
        {
            RefreshStat(statType, value);
        }

        private void RefreshAll()
        {
            RefreshStat(StatType.MaxHealth, playerStats.MaxHealth);
            RefreshStat(StatType.Attack, playerStats.Attack);
            RefreshStat(StatType.Defense, playerStats.Defense);
            RefreshStat(StatType.CritChance, playerStats.CritChance);
            RefreshStat(StatType.CritDamage, playerStats.CritDamage);
            RefreshStat(StatType.MoveSpeed, playerStats.MoveSpeed);
        }

        private void RefreshStat(StatType statType, float value)
        {
            switch (statType)
            {
                case StatType.MaxHealth:
                    maxHealthValueText.text = $"{value:0.#}";
                    break;

                case StatType.Attack:
                    attackValueText.text = $"{value:0.#}";
                    break;

                case StatType.Defense:
                    defenseValueText.text = $"{value:0.#}";
                    break;

                case StatType.CritChance:
                    critChanceValueText.text = $"{value * 100f:0.#}%";
                    break;

                case StatType.CritDamage:
                    critDamageValueText.text = $"{value * 100f:0.#}%";
                    break;

                case StatType.MoveSpeed:
                    moveSpeedValueText.text = $"{value:0.#}";
                    break;
            }
        }

        private bool HasAllReferences()
        {
            return playerStats != null &&
                   maxHealthValueText != null &&
                   attackValueText != null &&
                   defenseValueText != null &&
                   critChanceValueText != null &&
                   critDamageValueText != null &&
                   moveSpeedValueText != null;
        }
    }
}