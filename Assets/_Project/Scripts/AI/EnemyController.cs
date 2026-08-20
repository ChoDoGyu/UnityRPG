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
        private EliteCombatController eliteCombatController;
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
            eliteCombatController = GetComponent<EliteCombatController>();
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

            UpdateCombat(deltaTime);
            UpdateBehavior(deltaTime);

            visualAnimator.UpdateAnimation(
                enemyMotor.IsMoving,
                GetCurrentAttackPhase(),
                GetAttackPhaseProgress(),
                eliteCombatController != null &&
                eliteCombatController.IsSlamActionLocked,
                context.Definition.EnemyType,
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

            if (IsAttackActionLocked())
            {
                currentState = EnemyState.Attack;
                return;
            }

            if (ShouldEnterAttackState(target))
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

            if (eliteCombatController != null)
            {
                eliteCombatController.TryStartAttack(target);
                return;
            }

            if (!attackController.IsReady)
            {
                return;
            }

            attackController.TryStartAttack(target);
        }

        private void UpdateCombat(float deltaTime)
        {
            if (eliteCombatController != null)
            {
                eliteCombatController.UpdateCombat(deltaTime);
                return;
            }

            attackController.UpdateAttack(deltaTime);
        }

        private bool ShouldEnterAttackState(Transform target)
        {
            if (eliteCombatController != null)
            {
                return eliteCombatController.ShouldEnterAttackState(target);
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float attackRange = context.Definition.AttackRange;

            return direction.sqrMagnitude <= attackRange * attackRange;
        }

        private bool IsAttackActionLocked()
        {
            if (eliteCombatController != null)
            {
                return eliteCombatController.IsActionLocked;
            }

            return attackController.IsActionLocked;
        }

        private EnemyAttackPhase GetCurrentAttackPhase()
        {
            if (eliteCombatController != null)
            {
                return eliteCombatController.CurrentPhase;
            }

            return attackController.CurrentPhase;
        }

        private float GetAttackPhaseProgress()
        {
            if (eliteCombatController != null)
            {
                return eliteCombatController.PhaseNormalizedProgress;
            }

            return attackController.PhaseNormalizedProgress;
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