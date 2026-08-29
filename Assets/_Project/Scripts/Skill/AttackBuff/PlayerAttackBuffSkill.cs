using UnityEngine;
using UnityRPG.Character.Stats;
using UnityRPG.VFX;
using UnityRPG.Core;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(CombatVfxController))]
    public sealed class PlayerAttackBuffSkill : MonoBehaviour
    {
        [Header("Buff")]
        [SerializeField]
        [Min(0f)]
        private float attackBonus = 5f;

        [SerializeField]
        [Min(0.01f)]
        private float duration = 5f;

        [Header("Action")]
        [SerializeField]
        [Min(0.01f)]
        private float actionDuration = 0.3f;

        [Header("SFX")]
        [SerializeField] private AudioClip buffSfx;

        private PlayerStats playerStats;
        private CombatVfxController combatVfxController;

        private float remainingDuration;

        public bool IsActive => remainingDuration > 0f;

        public float ActionDuration => actionDuration;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            combatVfxController = GetComponent<CombatVfxController>();
        }

        private void OnDisable()
        {
            EndBuff();
        }

        public bool TryStart()
        {
            if (playerStats == null || !playerStats.IsConfigured)
            {
                return false;
            }

            if (IsActive)
            {
                return false;
            }

            playerStats.AddModifier(StatType.Attack, new StatModifier(attackBonus, this));
            remainingDuration = duration;

            combatVfxController.PlayAttackBuff();
            AudioService.Instance?.PlaySfx(buffSfx);

            return true;
        }

        public void UpdateSkill(float deltaTime)
        {
            if (!IsActive)
            {
                return;
            }

            if (deltaTime <= 0f)
            {
                return;
            }

            remainingDuration -= deltaTime;

            if (remainingDuration <= 0f)
            {
                EndBuff();
            }
        }

        private void EndBuff()
        {
            if (playerStats != null)
                playerStats.RemoveModifiersFromSource(this);

            if (combatVfxController != null)
                combatVfxController.StopAttackBuff();

            remainingDuration = 0f;
        }
    }
}