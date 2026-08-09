using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerAttackBuffSkill :
        MonoBehaviour
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

        private PlayerStats playerStats;
        private float remainingDuration;

        public bool IsActive =>
            remainingDuration > 0f;

        public float ActionDuration =>
            actionDuration;

        private void Awake()
        {
            playerStats =
                GetComponent<PlayerStats>();
        }

        private void OnDisable()
        {
            EndBuff();
        }

        public bool TryStart()
        {
            if (playerStats == null ||
                !playerStats.IsConfigured)
            {
                return false;
            }

            if (IsActive)
            {
                return false;
            }

            playerStats.AddModifier(
                StatType.Attack,
                new StatModifier(
                    attackBonus,
                    this));

            remainingDuration =
                duration;

            return true;
        }

        public void UpdateSkill(
            float deltaTime)
        {
            if (!IsActive)
            {
                return;
            }

            if (deltaTime <= 0f)
            {
                return;
            }

            remainingDuration -=
                deltaTime;

            if (remainingDuration <= 0f)
            {
                EndBuff();
            }
        }

        private void EndBuff()
        {
            if (playerStats != null)
            {
                playerStats.RemoveModifiersFromSource(
                    this);
            }

            remainingDuration = 0f;
        }
    }
}