using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyTargetDetector))]
    [RequireComponent(typeof(EnemyMotor))]
    public sealed class EnemyController : MonoBehaviour
    {
        [Header("Runtime State")]
        [SerializeField]
        private EnemyState currentState =
            EnemyState.Idle;

        private EnemyContext context;
        private EnemyTargetDetector targetDetector;
        private EnemyMotor enemyMotor;

        private bool isConfigured;

        public EnemyState CurrentState =>
            currentState;

        public Transform CurrentTarget =>
            targetDetector != null
                ? targetDetector.CurrentTarget
                : null;

        private void Awake()
        {
            context =
                GetComponent<EnemyContext>();

            targetDetector =
                GetComponent<EnemyTargetDetector>();

            enemyMotor = GetComponent<EnemyMotor>();

            if (!context.IsConfigured)
            {
                return;
            }

            currentState =
                EnemyState.Idle;

            isConfigured = true;
        }

        private void Update()
        {
            if (!isConfigured)
            {
                return;
            }

            targetDetector.UpdateDetection();

            UpdateState();

            UpdateBehavior();
        }

        private void UpdateState()
        {
            Transform target =
                targetDetector.CurrentTarget;

            if (target == null)
            {
                currentState =
                    EnemyState.Idle;

                return;
            }

            Vector3 direction =
                target.position -
                transform.position;

            direction.y = 0f;

            float attackRange =
                context.Definition.AttackRange;

            if (direction.sqrMagnitude <=
                attackRange *
                attackRange)
            {
                currentState =
                    EnemyState.Attack;

                return;
            }

            currentState =
                EnemyState.Chase;
        }

        private void UpdateBehavior()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    enemyMotor.Stop();
                    break;

                case EnemyState.Chase:
                    UpdateChase();
                    break;

                case EnemyState.Attack:
                    enemyMotor.Stop();
                    break;
            }
        }

        private void UpdateChase()
        {
            Transform target =
                targetDetector.CurrentTarget;

            if (target == null)
            {
                enemyMotor.Stop();

                return;
            }

            enemyMotor.TrySetDestination(
                target.position);
        }
    }
}