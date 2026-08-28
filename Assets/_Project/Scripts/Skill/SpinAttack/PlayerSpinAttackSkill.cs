using System.Collections.Generic;
using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.Character.Stats;
using UnityRPG.VFX;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(CombatVfxController))]
    public sealed class PlayerSpinAttackSkill : MonoBehaviour
    {
        [Header("Area")]
        [SerializeField, Min(0f)] private float attackRadius = 3f;

        [SerializeField]
        private LayerMask targetLayer;

        [Header("Damage")]
        [SerializeField, Min(0f)] private float damageMultiplier = 2.5f;

        [Header("Action")]
        [SerializeField, Min(0.01f)] private float actionDuration = 0.45f;

        [SerializeField, Range(0f, 1f)] private float hitNormalizedTime = 0.5f;

        private float remainingDuration;
        private bool isHitApplied;

        public float ActionDuration => actionDuration;

        private PlayerStats playerStats;
        private CombatVfxController combatVfxController;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            combatVfxController = GetComponent<CombatVfxController>();
        }

        public bool TryStart()
        {
            if (playerStats == null || !playerStats.IsConfigured)
            {
                return false;
            }

            if (remainingDuration > 0f)
            {
                return false;
            }

            remainingDuration = actionDuration;
            isHitApplied = false;

            combatVfxController.PlaySpinAttack();

            return true;
        }

        private void ApplyAreaDamage()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer, QueryTriggerInteraction.Ignore);

            HashSet<IDamageable> targets = new HashSet<IDamageable>();

            foreach (Collider hit in hits)
            {
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();

                if (damageable == null)
                {
                    continue;
                }

                if (!targets.Add(damageable))
                {
                    continue;
                }

                DamageInfo damageInfo =
                    DamageCalculator.CreateAttackDamage(playerStats.Attack, damageMultiplier, playerStats.CritChance, playerStats.CritDamage, gameObject);

                damageable.TakeDamage(damageInfo);

                Vector3 hitPoint = hit.ClosestPoint(transform.position);
                combatVfxController.PlayHit(hitPoint);
            }
        }

        public void UpdateSkill(float deltaTime)
        {
            if (remainingDuration <= 0f)
            {
                return;
            }

            if (deltaTime <= 0f)
            {
                return;
            }

            remainingDuration = Mathf.Max(0f, remainingDuration - deltaTime);

            float progress = 1f - remainingDuration / actionDuration;

            if (!isHitApplied && progress >= hitNormalizedTime)
            {
                ApplyAreaDamage();

                isHitApplied = true;
            }
        }
    }
}