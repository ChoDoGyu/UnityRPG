using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyTargetDetector : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField]
        private LayerMask targetLayer;

        [Header("Runtime")]
        [SerializeField]
        private Transform currentTarget;

        private readonly Collider[] detectionBuffer = new Collider[8];

        private EnemyContext context;
        private bool isConfigured;

        public Transform CurrentTarget => currentTarget;

        public bool HasTarget => currentTarget != null;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();

            if (!context.IsConfigured)
            {
                return;
            }

            if (targetLayer.value == 0)
            {
                Debug.LogError(
                    "[Enemy] EnemyTargetDetector의 Target Layer가 설정되지 않았습니다.",
                    this);

                return;
            }

            isConfigured = true;
        }

        public void UpdateDetection()
        {
            if (!isConfigured)
            {
                currentTarget = null;
                return;
            }

            if (currentTarget != null)
            {
                if (IsTargetAlive(currentTarget) &&
                    IsTargetInDetectionRange(currentTarget))
                {
                    return;
                }

                currentTarget = null;
            }

            FindTarget();
        }

        private void FindTarget()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                context.Definition.DetectionRange,
                detectionBuffer,
                targetLayer,
                QueryTriggerInteraction.Ignore);

            float closestSqrDistance = float.MaxValue;
            Transform closestTarget = null;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = detectionBuffer[i];

                if (hit == null)
                {
                    continue;
                }

                PlayerController player =
                    hit.GetComponentInParent<PlayerController>();

                if (player == null)
                {
                    continue;
                }

                if (!IsTargetAlive(player.transform))
                {
                    continue;
                }

                Vector3 direction =
                    player.transform.position -
                    transform.position;

                direction.y = 0f;

                float sqrDistance = direction.sqrMagnitude;

                if (sqrDistance >= closestSqrDistance)
                {
                    continue;
                }

                closestSqrDistance = sqrDistance;
                closestTarget = player.transform;
            }

            currentTarget = closestTarget;
        }

        private bool IsTargetAlive(Transform target)
        {
            if (target == null)
            {
                return false;
            }

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

            return playerHealth != null && !playerHealth.IsDead;
        }

        private bool IsTargetInDetectionRange(Transform target)
        {
            Vector3 direction =
                target.position -
                transform.position;

            direction.y = 0f;

            float detectionRange = context.Definition.DetectionRange;

            return direction.sqrMagnitude <=
                detectionRange * detectionRange;
        }
    }
}