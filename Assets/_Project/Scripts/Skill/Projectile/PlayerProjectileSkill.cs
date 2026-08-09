using UnityEngine;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    public sealed class PlayerProjectileSkill :
        MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private Transform directionReference;

        [SerializeField]
        private SkillProjectile projectilePrefab;

        [Header("Projectile")]
        [SerializeField]
        [Min(0.01f)]
        private float projectileSpeed = 12f;

        [SerializeField]
        [Min(0.01f)]
        private float projectileLifetime = 2.5f;

        [SerializeField]
        [Min(0f)]
        private float hitRadius = 0.2f;

        [SerializeField]
        private LayerMask collisionMask;

        [Header("Damage")]
        [SerializeField]
        [Min(0f)]
        private float damage = 20f;

        [Header("Action")]
        [SerializeField]
        [Min(0.01f)]
        private float actionDuration = 0.3f;

        public float ActionDuration =>
            actionDuration;

        private bool isConfigured;

        private void Awake()
        {
            if (spawnPoint == null ||
                directionReference == null ||
                projectilePrefab == null)
            {
                Debug.LogError(
                    "[Skill] Projectile Skill의 Reference가 설정되지 않았습니다.",
                    this);

                return;
            }

            isConfigured = true;
        }

        public bool TryStart()
        {
            if (!isConfigured)
            {
                return false;
            }

            Vector3 direction =
                directionReference.forward;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return false;
            }

            direction.Normalize();

            SkillProjectile projectile =
                Instantiate(
                    projectilePrefab,
                    spawnPoint.position,
                    Quaternion.LookRotation(
                        direction));

            projectile.Initialize(
                direction,
                projectileSpeed,
                projectileLifetime,
                hitRadius,
                collisionMask,
                damage,
                gameObject);

            return true;
        }
    }
}