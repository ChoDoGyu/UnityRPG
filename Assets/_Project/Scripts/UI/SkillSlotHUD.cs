using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Skill;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class SkillSlotHUD : MonoBehaviour
    {
        [Header("Skill")]
        [SerializeField] private SkillId skillId;
        [SerializeField] private string keyLabel;

        [Header("View")]
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TMP_Text keyText;
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text cooldownText;

        private RuntimeSkill runtimeSkill;
        private int lastCooldownTenths = -1;
        private bool wasReady = true;

        public SkillId SkillId => skillId;

        public void Initialize(RuntimeSkill skill)
        {
            runtimeSkill = skill;

            if (runtimeSkill == null)
                return;

            keyText.text = keyLabel;
            skillNameText.text = runtimeSkill.Definition.DisplayName;

            lastCooldownTenths = -1;
            wasReady = !runtimeSkill.IsReady;

            Refresh();
        }

        public void Refresh()
        {
            if (runtimeSkill == null)
                return;

            cooldownOverlay.fillAmount = runtimeSkill.CooldownNormalized;

            if (runtimeSkill.IsReady)
            {
                if (!wasReady)
                    cooldownText.text = string.Empty;

                wasReady = true;
                lastCooldownTenths = -1;
                return;
            }

            int cooldownTenths = Mathf.RoundToInt(runtimeSkill.RemainingCooldown * 10f);

            if (wasReady || cooldownTenths != lastCooldownTenths)
            {
                cooldownText.text = (cooldownTenths / 10f).ToString("0.0");
                lastCooldownTenths = cooldownTenths;
            }

            wasReady = false;
        }
    }
}