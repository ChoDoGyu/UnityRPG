using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyTargetDetector))]
    [RequireComponent(typeof(EnemyMotor))]
    [RequireComponent(typeof(EnemyAttackController))]
    [RequireComponent(typeof(EnemyVisualAnimator))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyController : MonoBehaviour
    {
        [Header("Runtime State")]
        [SerializeField]
        private EnemyState currentState = EnemyState.Idle;

        [Header("Death")]
        [SerializeField]
        [Min(0f)]
        private float corpseLifetime = 3f;

        private EnemyContext context;
        private EnemyTargetDetector targetDetector;
        private EnemyMotor enemyMotor;
        private EnemyAttackController attackController;
        private EnemyVisualAnimator visualAnimator;
        private EnemyHealth enemyHealth;
        private LockOnTarget lockOnTarget;
        private Collider bodyCollider;

        private bool isConfigured;
        private bool isDead;

        public EnemyState CurrentState => currentState;

        public Transform CurrentTarget =>
            targetDetector != null
                ? targetDetector.CurrentTarget
                : null;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
            targetDetector = GetComponent<EnemyTargetDetector>();
            enemyMotor = GetComponent<EnemyMotor>();
            attackController = GetComponent<EnemyAttackController>();
            visualAnimator = GetComponent<EnemyVisualAnimator>();
            enemyHealth = GetComponent<EnemyHealth>();
            lockOnTarget = GetComponent<LockOnTarget>();
            bodyCollider = GetComponent<Collider>();

            if (!context.IsConfigured)
            {
                return;
            }

            currentState = EnemyState.Idle;
            isConfigured = true;

            enemyHealth.Died += HandleDied;
        }

        private void Update()
        {
            if (!isConfigured || isDead)
            {
                return;
            }

            float deltaTime = Time.deltaTime;

            targetDetector.UpdateDetection();
            UpdateState();

            attackController.UpdateCooldown(deltaTime);

            UpdateBehavior(deltaTime);

            visualAnimator.UpdateAnimation(
                enemyMotor.IsMoving,
                deltaTime);
        }

        private void UpdateState()
        {
            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                currentState = EnemyState.Idle;
                return;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float attackRange = context.Definition.AttackRange;

            if (direction.sqrMagnitude <= attackRange * attackRange)
            {
                currentState = EnemyState.Attack;
                return;
            }

            currentState = EnemyState.Chase;
        }

        private void UpdateBehavior(float deltaTime)
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
                    UpdateAttackFacing(deltaTime);
                    UpdateAttack();
                    break;
            }
        }

        private void UpdateChase()
        {
            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                enemyMotor.Stop();
                return;
            }

            enemyMotor.TrySetDestination(target.position);
        }

        private void UpdateAttackFacing(float deltaTime)
        {
            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                return;
            }

            Vector3 direction = target.position - transform.position;

            enemyMotor.RotateTowards(
                direction,
                deltaTime);
        }

        private void UpdateAttack()
        {
            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                return;
            }

            if (!attackController.TryAttack(target))
            {
                return;
            }

            visualAnimator.PlayAttack(
                context.Definition.EnemyType);
        }

        private void OnDestroy()
        {
            if (enemyHealth != null)
            {
                enemyHealth.Died -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            currentState = EnemyState.Dead;

            enemyMotor.Disable();
            visualAnimator.PlayDeath();

            if (lockOnTarget != null)
            {
                lockOnTarget.enabled = false;
            }

            if (bodyCollider != null)
            {
                bodyCollider.enabled = false;
            }

            Destroy(gameObject, corpseLifetime);
        }
    }
}