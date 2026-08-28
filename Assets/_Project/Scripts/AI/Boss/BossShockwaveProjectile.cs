using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.VFX;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ProjectileVfxController))]
    public sealed class BossShockwaveProjectile : MonoBehaviour
    {
        private Vector3 direction;
        private DamageInfo damageInfo;

        private float speed;
        private float hitRadius;
        private float maximumDistance;
        private float travelledDistance;

        private LayerMask collisionMask;

        private bool hasHitTarget;
        private bool isInitialized;

        private ProjectileVfxController vfxController;

        private void Awake()
        {
            vfxController = GetComponent<ProjectileVfxController>();
        }

        public void Initialize(Vector3 direction, float speed, float hitRadius,
            float maximumDistance, DamageInfo damageInfo, LayerMask collisionMask)
        {
            this.direction = direction.normalized;
            this.speed = speed;
            this.hitRadius = hitRadius;
            this.maximumDistance = maximumDistance;
            this.damageInfo = damageInfo;
            this.collisionMask = collisionMask;

            travelledDistance = 0f;
            hasHitTarget = false;
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
                return;

            float remainingDistance = maximumDistance - travelledDistance;

            if (remainingDistance <= 0f)
            {
                FinishProjectile();
                return;
            }

            float moveDistance = Mathf.Min(speed * Time.deltaTime, remainingDistance);

            TryHitTarget(moveDistance);

            transform.position += direction * moveDistance;
            travelledDistance += moveDistance;

            if (travelledDistance >= maximumDistance)
                FinishProjectile();
        }

        private void TryHitTarget(float moveDistance)
        {
            if (hasHitTarget)
                return;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, hitRadius, direction,
                moveDistance, collisionMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable damageable = hits[i].collider.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                damageable.TakeDamage(damageInfo);

                Vector3 hitPoint = hits[i].collider.ClosestPoint(transform.position);
                vfxController.PlayImpact(hitPoint);

                hasHitTarget = true;
                return;
            }
        }

        private void FinishProjectile()
        {
            if (!isInitialized)
                return;

            isInitialized = false;

            float delay = vfxController != null ? vfxController.Finish(transform.position, false) : 0f;
            Destroy(gameObject, delay);
        }
    }
}