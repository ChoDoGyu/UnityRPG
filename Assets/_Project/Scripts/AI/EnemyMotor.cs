using UnityEngine;
using UnityEngine.AI;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyMotor : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField]
        private EnemyDefinition definition;

        private NavMeshAgent agent;
        private bool isConfigured;

        public bool IsMoving =>
            isConfigured &&
            agent.isOnNavMesh &&
            agent.velocity.sqrMagnitude > 0.01f;

        public Vector3 Velocity =>
            isConfigured
                ? agent.velocity
                : Vector3.zero;

        private void Awake()
        {
            agent =
                GetComponent<NavMeshAgent>();

            if (definition == null)
            {
                Debug.LogError(
                    "[Enemy] EnemyMotor의 Enemy Definition이 설정되지 않았습니다.",
                    this);

                return;
            }

            agent.speed =
                definition.MoveSpeed;

            isConfigured = true;
        }

        public bool TrySetDestination(
            Vector3 destination)
        {
            if (!isConfigured ||
                !agent.enabled ||
                !agent.isOnNavMesh)
            {
                return false;
            }

            agent.isStopped =
                false;

            return agent.SetDestination(
                destination);
        }

        public void Stop()
        {
            if (!isConfigured ||
                !agent.enabled ||
                !agent.isOnNavMesh)
            {
                return;
            }

            agent.isStopped =
                true;

            agent.ResetPath();
        }
    }
}