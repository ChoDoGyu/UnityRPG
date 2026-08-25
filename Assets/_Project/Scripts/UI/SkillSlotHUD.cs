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

        public SkillId SkillId => skillId;

        public void Initialize(RuntimeSkill skill)
        {
            runtimeSkill = skill;

            if (runtimeSkill == null)
                return;

            keyText.text = keyLabel;
            skillNameText.text = runtimeSkill.Definition.DisplayName;
            Refresh();
        }

        public void Refresh()
        {
            if (runtimeSkill == null)
                return;

            cooldownOverlay.fillAmount = runtimeSkill.CooldownNormalized;

            if (runtimeSkill.IsReady)
            {
                cooldownText.text = string.Empty;
                return;
            }

            cooldownText.text = $"{runtimeSkill.RemainingCooldown:0.0}";
        }
    }
}