using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.VFX;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ProjectileVfxController))]
    public sealed class BossShockwaveProjectile : MonoBehaviour
    {
        private readonly RaycastHit[] hitResults = new RaycastHit[16];

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

            int hitCount = Physics.SphereCastNonAlloc(
                transform.position,
                hitRadius,
                direction,
                hitResults,
                moveDistance,
                collisionMask,
                QueryTriggerInteraction.Ignore);

            IDamageable closestDamageable = null;
            Collider closestCollider = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hitResults[i];
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

                if (damageable == null || hit.distance >= closestDistance)
                    continue;

                closestDamageable = damageable;
                closestCollider = hit.collider;
                closestDistance = hit.distance;
            }

            if (closestDamageable == null)
                return;

            closestDamageable.TakeDamage(damageInfo);

            Vector3 hitPoint = closestCollider.ClosestPoint(transform.position);
            vfxController.PlayImpact(hitPoint);

            hasHitTarget = true;
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