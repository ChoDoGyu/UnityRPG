using UnityEngine;
using UnityRPG.Skill;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class PlayerSkillHUD : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerSkillController playerSkillController;

        [Header("Slots")]
        [SerializeField] private SkillSlotHUD dashSlashSlot;
        [SerializeField] private SkillSlotHUD projectileSlot;
        [SerializeField] private SkillSlotHUD spinAttackSlot;
        [SerializeField] private SkillSlotHUD attackBuffSlot;

        private bool isInitialized;

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] PlayerSkillHUD의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            if (!InitializeSlots())
            {
                Debug.LogError("[UI] PlayerSkillHUD의 RuntimeSkill을 찾을 수 없습니다.", this);
                enabled = false;
                return;
            }

            isInitialized = true;
        }

        private void LateUpdate()
        {
            if (!isInitialized)
                return;

            dashSlashSlot.Refresh();
            projectileSlot.Refresh();
            spinAttackSlot.Refresh();
            attackBuffSlot.Refresh();
        }

        private bool InitializeSlots()
        {
            RuntimeSkill dashSlash = playerSkillController.GetSkill(SkillId.DashSlash);
            RuntimeSkill projectile = playerSkillController.GetSkill(SkillId.Projectile);
            RuntimeSkill spinAttack = playerSkillController.GetSkill(SkillId.SpinAttack);
            RuntimeSkill attackBuff = playerSkillController.GetSkill(SkillId.AttackBuff);

            if (dashSlash == null || projectile == null || spinAttack == null || attackBuff == null)
                return false;

            dashSlashSlot.Initialize(dashSlash);
            projectileSlot.Initialize(projectile);
            spinAttackSlot.Initialize(spinAttack);
            attackBuffSlot.Initialize(attackBuff);

            return true;
        }

        private bool HasAllReferences()
        {
            return playerSkillController != null &&
                   dashSlashSlot != null &&
                   projectileSlot != null &&
                   spinAttackSlot != null &&
                   attackBuffSlot != null;
        }
    }
}