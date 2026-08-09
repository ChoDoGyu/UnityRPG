using UnityEngine;

namespace UnityRPG.Skill
{
    [CreateAssetMenu(
        fileName = "SkillDefinition",
        menuName = "UnityRPG/Skills/Skill Definition")]
    public sealed class SkillDefinition :
        ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private SkillId skillId;

        [SerializeField]
        private string displayName;

        [Header("Cooldown")]
        [SerializeField]
        [Min(0f)]
        private float cooldown = 5f;

        public SkillId SkillId =>
            skillId;

        public string DisplayName =>
            displayName;

        public float Cooldown =>
            cooldown;
    }
}