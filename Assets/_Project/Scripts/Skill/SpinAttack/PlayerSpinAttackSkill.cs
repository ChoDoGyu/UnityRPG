using System.Collections.Generic;
using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.Character.Stats;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerSpinAttackSkill :
        MonoBehaviour
    {
        [Header("Area")]
        [SerializeField]
        [Min(0f)]
        private float attackRadius = 3f;

        [SerializeField]
        private LayerMask targetLayer;

        [Header("Damage")]
        [SerializeField]
        [Min(0f)]
        private float damageMultiplier = 2.5f;

        [Header("Action")]
        [SerializeField]
        [Min(0.01f)]
        private float actionDuration = 0.45f;

        public float ActionDuration =>
            actionDuration;

        private PlayerStats playerStats;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
        }

        public bool TryStart()
        {
            ApplyAreaDamage();

            return true;
        }

        private void ApplyAreaDamage()
        {
            Collider[] hits =
                Physics.OverlapSphere(
                    transform.position,
                    attackRadius,
                    targetLayer,
                    QueryTriggerInteraction.Ignore);

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
                    DamageCalculator.CreateAttackDamage(
                        playerStats.Attack,
                        damageMultiplier,
                        playerStats.CritChance,
                        playerStats.CritDamage,
                        gameObject);

                damageable.TakeDamage(damageInfo);
            }
        }
    }
}