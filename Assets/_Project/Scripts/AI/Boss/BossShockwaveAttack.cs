using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class BossShockwaveAttack : BossPatternBase
    {
        [Header("Shockwave")]
        [SerializeField]
        [Min(0f)]
        private float minimumStartRange = 5f;

        [SerializeField]
        [Min(0f)]
        private float maximumStartRange = 12f;

        [SerializeField]
        [Min(0.01f)]
        private float projectileSpeed = 10f;

        [SerializeField]
        [Min(0.01f)]
        private float maximumDistance = 15f;

        [SerializeField]
        [Min(0f)]
        private float hitRadius = 0.8f;

        [SerializeField]
        [Min(0f)]
        private float damageMultiplier = 1.3f;

        [Header("Reference")]
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private BossShockwaveProjectile projectilePrefab;

        [SerializeField]
        private LayerMask collisionMask;

        protected override BossPatternType PatternType => BossPatternType.Shockwave;

        protected override bool IsAvailableInPhase(BossPhase bossPhase)
        {
            return bossPhase == BossPhase.Phase2;
        }

        protected override bool ValidatePatternConfiguration()
        {
            return spawnPoint != null && projectilePrefab != null && collisionMask.value != 0;
        }

        protected override bool CanStartPattern(Transform target)
        {
            Vector3 direction = target.position - transform.position;

            direction.y = 0f;

            float sqrDistance = direction.sqrMagnitude;

            return sqrDistance >= minimumStartRange * minimumStartRange &&
                   sqrDistance <= maximumStartRange * maximumStartRange;
        }

        protected override void OnActiveStarted()
        {
            FireShockwave();
        }

        private void FireShockwave()
        {
            if (CurrentTarget == null)
            {
                return;
            }

            Vector3 direction = CurrentTarget.position - spawnPoint.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            direction.Normalize();

            DamageInfo damageInfo = CreateDamageInfo(damageMultiplier);

            BossShockwaveProjectile projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(direction));

            projectile.Initialize(direction, projectileSpeed, hitRadius, maximumDistance, damageInfo, collisionMask);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            minimumStartRange = Mathf.Max(0f, minimumStartRange);

            maximumStartRange = Mathf.Max(minimumStartRange, maximumStartRange);

            projectileSpeed = Mathf.Max(0.01f, projectileSpeed);
            maximumDistance = Mathf.Max(0.1f, maximumDistance);
            hitRadius = Mathf.Max(0f, hitRadius);
            damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }
    }
}