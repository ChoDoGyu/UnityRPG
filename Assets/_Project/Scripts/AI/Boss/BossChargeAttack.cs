using UnityEngine;
using UnityRPG.Core;
using UnityRPG.VFX;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyMotor))]
    public sealed class BossChargeAttack : BossPatternBase
    {
        [Header("Charge")]
        [SerializeField, Min(0f)] private float minimumStartRange = 4f;
        [SerializeField, Min(0f)] private float maximumStartRange = 10f;
        [SerializeField, Min(0.01f)] private float chargeSpeed = 12f;
        [SerializeField, Min(0.01f)] private float maximumChargeDistance = 10f;
        [SerializeField, Min(0f)] private float hitRange = 2.5f;
        [SerializeField, Min(0f)] private float damageMultiplier = 1.6f;

        [Header("Telegraph")]
        [SerializeField] private GameObject telegraphObject;

        [Header("VFX")]
        [SerializeField] private GameObject chargeVfxPrefab;
        [SerializeField] private GameObject impactVfxPrefab;
        [SerializeField, Min(0f)] private float chargeVfxCleanupDelay = 0.7f;

        [Header("SFX")]
        [SerializeField] private AudioClip hitSfx;

        private EnemyMotor enemyMotor;
        private GameObject activeChargeVfx;

        private Vector3 chargeDirection;
        private float travelledDistance;
        private bool hasHitTarget;

        protected override BossPatternType PatternType => BossPatternType.Charge;

        public override bool ShouldTrackTargetRotation => CurrentPhase != BossPatternPhase.Active;
        public override bool ShouldStopMotor => CurrentPhase != BossPatternPhase.Active;

        protected override void Awake()
        {
            enemyMotor = GetComponent<EnemyMotor>();

            base.Awake();
        }

        protected override bool ValidatePatternConfiguration()
        {
            return telegraphObject != null;
        }

        protected override void OnPatternInitialized()
        {
            telegraphObject.SetActive(false);
        }

        protected override bool CanStartPattern(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float sqrDistance = direction.sqrMagnitude;

            return sqrDistance >= minimumStartRange * minimumStartRange &&
                   sqrDistance <= maximumStartRange * maximumStartRange;
        }

        protected override void OnWindupStarted()
        {
            telegraphObject.SetActive(true);

            travelledDistance = 0f;
            hasHitTarget = false;
        }

        protected override void OnActiveStarted()
        {
            telegraphObject.SetActive(false);

            activeChargeVfx = VfxSpawner.SpawnAttached(chargeVfxPrefab, transform);

            chargeDirection = transform.forward;
            chargeDirection.y = 0f;

            if (chargeDirection.sqrMagnitude <= 0.001f)
            {
                CompleteActive();
                return;
            }

            chargeDirection.Normalize();

            travelledDistance = 0f;
            hasHitTarget = false;
        }

        protected override void OnActiveUpdated(float deltaTime)
        {
            if (travelledDistance >= maximumChargeDistance)
            {
                CompleteActive();
                return;
            }

            float remainingDistance = maximumChargeDistance - travelledDistance;
            float moveDistance = Mathf.Min(chargeSpeed * deltaTime, remainingDistance);

            if (!enemyMotor.TryMove(chargeDirection * moveDistance))
            {
                CompleteActive();
                return;
            }

            travelledDistance += moveDistance;

            if (!hasHitTarget && TryHitTarget())
            {
                hasHitTarget = true;
                CompleteActive();
                return;
            }

            if (travelledDistance >= maximumChargeDistance)
                CompleteActive();
        }

        protected override void OnRecoveryStarted()
        {
            StopChargeVfx();
        }

        protected override void OnPatternCancelled()
        {
            if (telegraphObject != null)
                telegraphObject.SetActive(false);

            StopChargeVfx();

            chargeDirection = Vector3.zero;
            travelledDistance = 0f;
            hasHitTarget = false;
        }

        private bool TryHitTarget()
        {
            if (CurrentTarget == null)
                return false;

            Vector3 direction = CurrentTarget.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > hitRange * hitRange)
                return false;

            if (!TryApplyDamage(CurrentTarget, damageMultiplier))
                return false;

            Collider targetCollider = CurrentTarget.GetComponentInParent<Collider>();
            Vector3 hitPoint = targetCollider != null ?
                targetCollider.ClosestPoint(transform.position) :
                CurrentTarget.position;

            VfxSpawner.Spawn(impactVfxPrefab, hitPoint, Quaternion.identity);
            AudioService.Instance?.PlaySfx(hitSfx);

            return true;
        }

        private void StopChargeVfx()
        {
            if (activeChargeVfx == null)
                return;

            ParticleSystem[] particleSystems =
                activeChargeVfx.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < particleSystems.Length; i++)
                particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);

            Destroy(activeChargeVfx, chargeVfxCleanupDelay);
            activeChargeVfx = null;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            minimumStartRange = Mathf.Max(0f, minimumStartRange);
            maximumStartRange = Mathf.Max(minimumStartRange, maximumStartRange);
            chargeSpeed = Mathf.Max(0.01f, chargeSpeed);
            maximumChargeDistance = Mathf.Max(0.01f, maximumChargeDistance);
            hitRange = Mathf.Max(0f, hitRange);
            damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }
    }
}