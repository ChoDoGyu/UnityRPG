using UnityEngine;
using UnityEngine.AI;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyMotor :
        MonoBehaviour
    {
        private NavMeshAgent agent;
        private EnemyContext context;

        private bool isConfigured;

        public bool IsMoving =>
            isConfigured &&
            agent.enabled &&
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

            context =
                GetComponent<EnemyContext>();

            if (!context.IsConfigured)
            {
                return;
            }

            agent.speed =
                context.Definition.MoveSpeed;

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