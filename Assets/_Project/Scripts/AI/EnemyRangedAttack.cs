using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyRangedAttack : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private EnemyProjectile projectilePrefab;

        [Header("Projectile")]
        [SerializeField]
        [Min(0.01f)]
        private float projectileSpeed = 10f;

        [SerializeField]
        [Min(0.01f)]
        private float projectileLifetime = 3f;

        [SerializeField]
        [Min(0f)]
        private float hitRadius = 0.2f;

        [SerializeField]
        private LayerMask collisionMask;

        private EnemyContext context;
        private bool isConfigured;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();

            if (!context.IsConfigured || spawnPoint == null || projectilePrefab == null || collisionMask.value == 0)
            {
                Debug.LogError("[Enemy] EnemyRangedAttack의 설정이 올바르지 않습니다.", this);

                return;
            }

            isConfigured = true;
        }

        public bool TryAttack(Transform target)
        {
            if (!isConfigured || target == null)
            {
                return false;
            }

            Vector3 targetPosition = target.position;

            CharacterController targetController = target.GetComponent<CharacterController>();

            if (targetController != null)
            {
                targetPosition = target.TransformPoint(targetController.center);
            }

            Vector3 direction = targetPosition - spawnPoint.position;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return false;
            }

            direction.Normalize();

            DamageInfo damageInfo = new DamageInfo(context.Definition.Attack, gameObject);

            EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(direction));

            projectile.Initialize(direction, projectileSpeed, projectileLifetime, hitRadius, collisionMask, damageInfo);

            return true;
        }
    }
}