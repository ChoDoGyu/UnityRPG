using System;

namespace UnityRPG.Skill
{
    public sealed class RuntimeSkill
    {
        public SkillDefinition Definition { get; }

        public float RemainingCooldown { get; private set; }

        public bool IsReady => RemainingCooldown <= 0f;

        public float CooldownNormalized
        {
            get
            {
                if (Definition.Cooldown <= 0f)
                    return 0f;

                return RemainingCooldown / Definition.Cooldown;
            }
        }

        public RuntimeSkill(SkillDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            RemainingCooldown = 0f;
        }

        public bool TryStartCooldown()
        {
            if (!IsReady)
                return false;

            RemainingCooldown = Definition.Cooldown;
            return true;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (IsReady || deltaTime <= 0f)
                return;

            RemainingCooldown -= deltaTime;

            if (RemainingCooldown < 0f)
                RemainingCooldown = 0f;
        }
    }
}