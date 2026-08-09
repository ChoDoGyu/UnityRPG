using System;
using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    public sealed class SkillProjectile : MonoBehaviour
    {
        private Vector3 direction;
        private float speed;
        private float remainingLifetime;
        private float hitRadius;
        private float damage;
        private LayerMask collisionMask;
        private GameObject source;

        private bool isInitialized;

        public void Initialize(
            Vector3 direction,
            float speed,
            float lifetime,
            float hitRadius,
            LayerMask collisionMask,
            float damage,
            GameObject source)
        {
            this.direction =
                direction.normalized;

            this.speed =
                speed;

            remainingLifetime =
                lifetime;

            this.hitRadius =
                hitRadius;

            this.collisionMask =
                collisionMask;

            this.damage =
                damage;

            this.source =
                source;

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            float deltaTime =
                Time.deltaTime;

            float moveDistance =
                speed * deltaTime;

            if (MoveProjectile(moveDistance))
            {
                return;
            }

            remainingLifetime -=
                deltaTime;

            if (remainingLifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private bool MoveProjectile(
            float moveDistance)
        {
            RaycastHit[] hits =
                Physics.SphereCastAll(
                    transform.position,
                    hitRadius,
                    direction,
                    moveDistance,
                    collisionMask,
                    QueryTriggerInteraction.Ignore);

            Array.Sort(
                hits,
                (a, b) =>
                    a.distance.CompareTo(
                        b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (IsSourceCollider(
                        hit.collider))
                {
                    continue;
                }

                transform.position =
                    hit.point -
                    direction * hitRadius;

                IDamageable damageable =
                    hit.collider
                        .GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(
                        new DamageInfo(
                            damage,
                            source));
                }

                Destroy(gameObject);

                return true;
            }

            transform.position +=
                direction * moveDistance;

            return false;
        }

        private bool IsSourceCollider(
            Collider collider)
        {
            if (source == null)
            {
                return false;
            }

            Transform colliderTransform =
                collider.transform;

            return
                colliderTransform ==
                source.transform ||
                colliderTransform.IsChildOf(
                    source.transform);
        }
    }
}