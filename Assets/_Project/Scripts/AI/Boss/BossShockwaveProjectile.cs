using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
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
                Destroy(gameObject);
                return;
            }

            float moveDistance = Mathf.Min(speed * Time.deltaTime, remainingDistance);

            TryHitTarget(moveDistance);

            transform.position += direction * moveDistance;
            travelledDistance += moveDistance;

            if (travelledDistance >= maximumDistance)
                Destroy(gameObject);
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
                hasHitTarget = true;
                return;
            }
        }
    }
}