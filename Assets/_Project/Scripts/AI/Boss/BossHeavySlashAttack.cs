using UnityEngine;
using UnityRPG.VFX;
using UnityRPG.Core;

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

        [Header("VFX")]
        [SerializeField] private GameObject windupVfxPrefab;
        [SerializeField] private GameObject slashVfxPrefab;
        [SerializeField] private GameObject impactVfxPrefab;
        [SerializeField, Min(0f)] private float vfxHeight = 0.8f;

        [Header("SFX")]
        [SerializeField] private AudioClip heavySlashSfx;

        private GameObject activeWindupVfx;

        protected override BossPatternType PatternType => BossPatternType.HeavySlash;

        protected override bool CanStartPattern(Transform target)
        {
            Vector3 direction = target.position - transform.position;

            direction.y = 0f;

            return direction.sqrMagnitude <= range * range;
        }

        protected override void OnActiveStarted()
        {
            ClearWindupVfx();

            Vector3 position = transform.position + Vector3.up * vfxHeight;

            VfxSpawner.Spawn(slashVfxPrefab, position, transform.rotation);
            AudioService.Instance?.PlaySfx(heavySlashSfx);

            ExecuteHit();
        }

        private void ExecuteHit()
        {
            if (CurrentTarget == null)
            {
                return;
            }

            Vector3 direction = CurrentTarget.position - transform.position;

            direction.y = 0f;

            float sqrDistance = direction.sqrMagnitude;

            if (sqrDistance > range * range || sqrDistance <= 0.001f)
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

            float minimumDot = Mathf.Cos(hitAngle * 0.5f * Mathf.Deg2Rad);

            if (Vector3.Dot(forward, direction) < minimumDot)
            {
                return;
            }

            if (!TryApplyDamage(CurrentTarget, damageMultiplier))
                return;

            Collider targetCollider = CurrentTarget.GetComponentInParent<Collider>();
            Vector3 hitPoint = targetCollider != null ?
                targetCollider.ClosestPoint(transform.position) :
                CurrentTarget.position;

            VfxSpawner.Spawn(impactVfxPrefab, hitPoint, Quaternion.identity);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            range = Mathf.Max(0f, range);
            hitAngle = Mathf.Clamp(hitAngle, 1f, 180f);
            damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }

        protected override void OnWindupStarted()
        {
            activeWindupVfx = VfxSpawner.SpawnAttached(windupVfxPrefab, transform);
        }

        protected override void OnPatternCancelled()
        {
            ClearWindupVfx();
        }

        private void ClearWindupVfx()
        {
            if (activeWindupVfx == null)
                return;

            Destroy(activeWindupVfx);
            activeWindupVfx = null;
        }
    }
}