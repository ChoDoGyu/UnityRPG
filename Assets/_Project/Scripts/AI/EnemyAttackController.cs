using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyAttackController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField]
        private EnemyAttackPhase currentPhase = EnemyAttackPhase.Ready;

        private EnemyContext context;
        private EnemyMeleeAttack meleeAttack;
        private EnemyRangedAttack rangedAttack;

        private Transform currentTarget;
        private float remainingPhaseTime;
        private bool isConfigured;

        public EnemyAttackPhase CurrentPhase => currentPhase;

        public float PhaseNormalizedProgress
        {
            get
            {
                if (!isConfigured)
                {
                    return 0f;
                }

                float duration;

                switch (currentPhase)
                {
                    case EnemyAttackPhase.Windup:
                        duration = context.Definition.AttackWindup;
                        break;

                    case EnemyAttackPhase.Recovery:
                        duration = context.Definition.AttackRecovery;
                        break;

                    case EnemyAttackPhase.Cooldown:
                        duration = context.Definition.AttackCooldown;
                        break;

                    default:
                        return 0f;
                }

                if (duration <= 0f)
                {
                    return 1f;
                }

                return 1f - Mathf.Clamp01(remainingPhaseTime / duration);
            }
        }

        public bool IsReady => currentPhase == EnemyAttackPhase.Ready;

        public bool IsActionLocked =>
            currentPhase == EnemyAttackPhase.Windup ||
            currentPhase == EnemyAttackPhase.Recovery;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
            meleeAttack = GetComponent<EnemyMeleeAttack>();
            rangedAttack = GetComponent<EnemyRangedAttack>();

            if (!context.IsConfigured)
            {
                return;
            }

            switch (context.Definition.EnemyType)
            {
                case EnemyType.Melee:
                    if (meleeAttack == null || !meleeAttack.isActiveAndEnabled)
                    {
                        Debug.LogError(
                            "[Enemy] Melee Enemy의 EnemyMeleeAttack이 없거나 비활성화되어 있습니다.",
                            this);

                        return;
                    }

                    break;

                case EnemyType.Ranged:
                    if (rangedAttack == null || !rangedAttack.isActiveAndEnabled)
                    {
                        Debug.LogError(
                            "[Enemy] Ranged Enemy의 EnemyRangedAttack이 없거나 비활성화되어 있습니다.",
                            this);

                        return;
                    }

                    break;
            }

            isConfigured = true;
        }

        public bool TryStartAttack(Transform target)
        {
            if (!isConfigured || !IsReady || target == null)
            {
                return false;
            }

            currentTarget = target;
            currentPhase = EnemyAttackPhase.Windup;
            remainingPhaseTime = context.Definition.AttackWindup;

            return true;
        }

        public void UpdateAttack(float deltaTime)
        {
            if (!isConfigured ||
                currentPhase == EnemyAttackPhase.Ready ||
                deltaTime <= 0f)
            {
                return;
            }

            remainingPhaseTime = Mathf.Max(0f, remainingPhaseTime - deltaTime);

            if (remainingPhaseTime > 0f)
            {
                return;
            }

            switch (currentPhase)
            {
                case EnemyAttackPhase.Windup:
                    ExecuteAttack();
                    StartRecovery();
                    break;

                case EnemyAttackPhase.Recovery:
                    StartCooldown();
                    break;

                case EnemyAttackPhase.Cooldown:
                    FinishAttackCycle();
                    break;
            }
        }

        private void ExecuteAttack()
        {
            if (currentTarget == null)
            {
                return;
            }

            switch (context.Definition.EnemyType)
            {
                case EnemyType.Melee:
                    if (meleeAttack != null && meleeAttack.isActiveAndEnabled)
                    {
                        meleeAttack.TryAttack(currentTarget);
                    }

                    break;

                case EnemyType.Ranged:
                    if (rangedAttack != null && rangedAttack.isActiveAndEnabled)
                    {
                        rangedAttack.TryAttack(currentTarget);
                    }

                    break;
            }
        }

        private void StartRecovery()
        {
            currentPhase = EnemyAttackPhase.Recovery;
            remainingPhaseTime = context.Definition.AttackRecovery;
        }

        private void StartCooldown()
        {
            currentPhase = EnemyAttackPhase.Cooldown;
            remainingPhaseTime = context.Definition.AttackCooldown;
            currentTarget = null;
        }

        private void FinishAttackCycle()
        {
            currentPhase = EnemyAttackPhase.Ready;
            remainingPhaseTime = 0f;
        }
    }
}