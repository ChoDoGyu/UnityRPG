using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerDashSlashSkill))]
    [RequireComponent(typeof(PlayerProjectileSkill))]
    public sealed class PlayerSkillController : MonoBehaviour
    {
        [Header("Skill Definitions")]
        [SerializeField]
        private SkillDefinition dashSlashDefinition;

        [SerializeField]
        private SkillDefinition projectileDefinition;

        [SerializeField]
        private SkillDefinition spinAttackDefinition;

        [SerializeField]
        private SkillDefinition attackBuffDefinition;

        private readonly Dictionary<SkillId, RuntimeSkill> skills =
            new Dictionary<SkillId, RuntimeSkill>();

        private bool isConfigured;

        private PlayerDashSlashSkill dashSlashSkill;
        private PlayerProjectileSkill projectileSkill;

        public bool BlocksMovement =>
            dashSlashSkill.IsActive;

        private void Awake()
        {
            if (!ValidateDefinitions())
            {
                return;
            }

            dashSlashSkill = GetComponent<PlayerDashSlashSkill>();
            projectileSkill = GetComponent<PlayerProjectileSkill>();

            AddRuntimeSkill(
                dashSlashDefinition);

            AddRuntimeSkill(
                projectileDefinition);

            AddRuntimeSkill(
                spinAttackDefinition);

            AddRuntimeSkill(
                attackBuffDefinition);

            isConfigured = true;
        }

        public void UpdateSkills(
            float deltaTime)
        {
            if (!isConfigured)
            {
                return;
            }

            foreach (RuntimeSkill skill in skills.Values)
            {
                skill.UpdateCooldown(
                    deltaTime);
            }

            dashSlashSkill.UpdateSkill(
                deltaTime);
        }

        public bool TryUseSkill(
            SkillId skillId)
        {
            if (!isConfigured)
            {
                return false;
            }

            if (!skills.TryGetValue(
                    skillId,
                    out RuntimeSkill skill))
            {
                return false;
            }

            if (!skill.IsReady)
            {
                return false;
            }

            if (skillId == SkillId.DashSlash)
            {
                if (!dashSlashSkill.TryStart())
                {
                    return false;
                }
            }

            if (skillId == SkillId.Projectile)
            {
                if (!projectileSkill.TryStart())
                {
                    return false;
                }
            }

            return skill.TryStartCooldown();
        }

        public RuntimeSkill GetSkill(
            SkillId skillId)
        {
            if (!isConfigured)
            {
                return null;
            }

            skills.TryGetValue(
                skillId,
                out RuntimeSkill skill);

            return skill;
        }

        private void AddRuntimeSkill(
            SkillDefinition definition)
        {
            skills.Add(
                definition.SkillId,
                new RuntimeSkill(
                    definition));
        }

        private bool ValidateDefinitions()
        {
            if (dashSlashDefinition == null ||
                projectileDefinition == null ||
                spinAttackDefinition == null ||
                attackBuffDefinition == null)
            {
                Debug.LogError(
                    "[Skill] PlayerSkillController의 Skill Definition이 설정되지 않았습니다.",
                    this);

                return false;
            }

            if (dashSlashDefinition.SkillId != SkillId.DashSlash ||
                projectileDefinition.SkillId != SkillId.Projectile ||
                spinAttackDefinition.SkillId != SkillId.SpinAttack ||
                attackBuffDefinition.SkillId != SkillId.AttackBuff)
            {
                Debug.LogError(
                    "[Skill] PlayerSkillController의 Skill Definition과 SkillId가 일치하지 않습니다.",
                    this);

                return false;
            }

            return true;
        }
    }
}