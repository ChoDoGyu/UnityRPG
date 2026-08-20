using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyTargetDetector))]
    [RequireComponent(typeof(EnemyMotor))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(BossCombatController))]
    [RequireComponent(typeof(BossVisualAnimator))]
    public sealed class BossController : MonoBehaviour
    {
        [Header("Runtime State")]
        [SerializeField]
        private EnemyState currentState = EnemyState.Idle;

        [Header("Death")]
        [SerializeField]
        [Min(0f)]
        private float corpseLifetime = 3f;

        [Header("Chase")]
        [SerializeField, Min(0f)] private float chaseStopDistance = 2.3f;

        private EnemyContext context;
        private EnemyTargetDetector targetDetector;
        private EnemyMotor enemyMotor;
        private EnemyHealth enemyHealth;
        private BossCombatController combatController;
        private BossVisualAnimator visualAnimator;

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
            enemyHealth = GetComponent<EnemyHealth>();
            combatController = GetComponent<BossCombatController>();
            visualAnimator = GetComponent<BossVisualAnimator>();

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
            combatController.UpdateCombat(deltaTime);

            TryStartPattern();

            UpdateState();
            UpdateBehavior(deltaTime);

            visualAnimator.UpdateAnimation(
                enemyMotor.IsMoving,
                combatController.CurrentPattern,
                combatController.CurrentPatternPhase,
                combatController.CurrentPatternProgress,
                deltaTime);
        }

        private void TryStartPattern()
        {
            if (combatController.HasActivePattern)
            {
                return;
            }

            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                return;
            }

            combatController.TryStartPattern(target);
        }

        private void UpdateState()
        {
            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                currentState = EnemyState.Idle;
                return;
            }

            if (combatController.HasActivePattern)
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
                    if (combatController.IsPatternIntervalActive)
                    {
                        enemyMotor.Stop();
                        UpdateAttackFacing(deltaTime);
                        break;
                    }

                    UpdateChase(deltaTime);
                    break;

                case EnemyState.Attack:
                    if (combatController.ShouldStopMotor)
                        enemyMotor.Stop();

                    if (combatController.ShouldTrackTargetRotation)
                        UpdateAttackFacing(deltaTime);

                    break;
            }
        }

        private void UpdateChase(float deltaTime)
        {
            Transform target = targetDetector.CurrentTarget;

            if (target == null)
            {
                enemyMotor.Stop();
                return;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= chaseStopDistance * chaseStopDistance)
            {
                enemyMotor.Stop();
                enemyMotor.RotateTowards(direction, deltaTime);
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

            Vector3 direction =
                target.position - transform.position;

            enemyMotor.RotateTowards(
                direction,
                deltaTime);
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