using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class BossGroundSlamAttack : BossPatternBase
    {
        [Header("Ground Slam")]
        [SerializeField]
        [Min(0f)]
        private float range = 4.5f;

        [SerializeField]
        [Min(0f)]
        private float damageMultiplier = 1.8f;

        [Header("Telegraph")]
        [SerializeField]
        private GameObject telegraphObject;

        protected override BossPatternType PatternType => BossPatternType.GroundSlam;

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

            return direction.sqrMagnitude <= range * range;
        }

        protected override void OnWindupStarted()
        {
            telegraphObject.SetActive(true);
        }

        protected override void OnActiveStarted()
        {
            telegraphObject.SetActive(false);

            ExecuteHit();
        }

        protected override void OnPatternCancelled()
        {
            if (telegraphObject != null)
            {
                telegraphObject.SetActive(false);
            }
        }

        private void ExecuteHit()
        {
            if (CurrentTarget == null)
            {
                return;
            }

            Vector3 direction = CurrentTarget.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > range * range)
            {
                return;
            }

            TryApplyDamage(CurrentTarget, damageMultiplier);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            range = Mathf.Max(0f, range);
            damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }
    }
}