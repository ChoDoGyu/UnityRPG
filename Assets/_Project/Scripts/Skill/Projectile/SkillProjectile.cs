using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.VFX;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ProjectileVfxController))]
    public sealed class SkillProjectile : MonoBehaviour
    {
        private readonly RaycastHit[] hitResults = new RaycastHit[16];

        private Vector3 direction;
        private float speed;
        private float remainingLifetime;
        private float hitRadius;
        private LayerMask collisionMask;
        private DamageInfo damageInfo;

        private ProjectileVfxController vfxController;

        private bool isInitialized;

        private void Awake()
        {
            vfxController = GetComponent<ProjectileVfxController>();
        }

        public void Initialize(Vector3 direction, float speed, float lifetime, float hitRadius,
            LayerMask collisionMask, DamageInfo damageInfo)
        {
            this.direction = direction.normalized;
            this.speed = speed;
            remainingLifetime = lifetime;
            this.hitRadius = hitRadius;
            this.collisionMask = collisionMask;
            this.damageInfo = damageInfo;

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
                return;

            float deltaTime = Time.deltaTime;
            float moveDistance = speed * deltaTime;

            if (MoveProjectile(moveDistance))
                return;

            remainingLifetime -= deltaTime;

            if (remainingLifetime <= 0f)
                FinishProjectile(transform.position, false);
        }

        private bool MoveProjectile(float moveDistance)
        {
            int hitCount = Physics.SphereCastNonAlloc(
                transform.position,
                hitRadius,
                direction,
                hitResults,
                moveDistance,
                collisionMask,
                QueryTriggerInteraction.Ignore);

            RaycastHit closestHit = default;
            float closestDistance = float.MaxValue;
            bool hasHit = false;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hitResults[i];

                if (IsSourceCollider(hit.collider) || hit.distance >= closestDistance)
                    continue;

                closestHit = hit;
                closestDistance = hit.distance;
                hasHit = true;
            }

            if (hasHit)
            {
                transform.position = closestHit.point - direction * hitRadius;

                IDamageable damageable = closestHit.collider.GetComponentInParent<IDamageable>();

                if (damageable != null)
                    damageable.TakeDamage(damageInfo);

                FinishProjectile(closestHit.point, true);
                return true;
            }

            transform.position += direction * moveDistance;
            return false;
        }

        private bool IsSourceCollider(Collider collider)
        {
            GameObject source = damageInfo.Source;

            if (source == null)
                return false;

            Transform colliderTransform = collider.transform;

            return colliderTransform == source.transform || colliderTransform.IsChildOf(source.transform);
        }

        private void FinishProjectile(Vector3 position, bool playImpact)
        {
            if (!isInitialized)
                return;

            isInitialized = false;

            float delay = vfxController != null ? vfxController.Finish(position, playImpact) : 0f;
            Destroy(gameObject, delay);
        }
    }
}