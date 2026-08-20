using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class BossHeavySlashAttack : BossPatternBase
    {
        [Header("Heavy Slash")]
        [SerializeField]
        [Min(0f)]
        private float range = 3f;

        [SerializeField]
        [Range(1f, 180f)]
        private float hitAngle = 100f;

        [SerializeField]
        [Min(0f)]
        private float damageMultiplier = 1.5f;

        protected override BossPatternType PatternType =>
            BossPatternType.HeavySlash;

        protected override bool CanStartPattern(Transform target)
        {
            Vector3 direction =
                target.position - transform.position;

            direction.y = 0f;

            return direction.sqrMagnitude <= range * range;
        }

        protected override void OnActiveStarted()
        {
            ExecuteHit();
        }

        private void ExecuteHit()
        {
            if (CurrentTarget == null)
            {
                return;
            }

            Vector3 direction =
                CurrentTarget.position - transform.position;

            direction.y = 0f;

            float sqrDistance = direction.sqrMagnitude;

            if (sqrDistance > range * range ||
                sqrDistance <= 0.001f)
            {
                return;
            }

            direction.Normalize();

            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.001f)
            {
                return;
            }

            forward.Normalize();

            float minimumDot =
                Mathf.Cos(
                    hitAngle *
                    0.5f *
                    Mathf.Deg2Rad);

            if (Vector3.Dot(forward, direction) < minimumDot)
            {
                return;
            }

            TryApplyDamage(CurrentTarget, damageMultiplier);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            range = Mathf.Max(0f, range);
            hitAngle = Mathf.Clamp(hitAngle, 1f, 180f);
            damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }
    }
}