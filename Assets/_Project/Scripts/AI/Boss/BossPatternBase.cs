using UnityEngine;
using UnityRPG.Character.Player;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(BossCombatController))]
    public abstract class BossPatternBase : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField]
        [Min(0f)]
        private float windup;

        [SerializeField]
        [Min(0.01f)]
        private float activeDuration = 0.1f;

        [SerializeField]
        [Min(0f)]
        private float recovery;

        [SerializeField]
        [Min(0f)]
        private float cooldown;

        [Header("Runtime")]
        [SerializeField]
        private BossPatternPhase currentPhase = BossPatternPhase.Ready;

        private EnemyContext context;
        private BossCombatController combatController;

        private Transform currentTarget;
        private float remainingPhaseTime;
        private bool isConfigured;

        protected abstract BossPatternType PatternType { get; }

        protected EnemyContext Context => context;
        protected Transform CurrentTarget => currentTarget;

        public BossPatternType Type => PatternType;

        public BossPatternPhase CurrentPhase => currentPhase;

        public bool IsReady => isConfigured && currentPhase == BossPatternPhase.Ready;

        public bool IsActionLocked =>
            currentPhase == BossPatternPhase.Windup ||
            currentPhase == BossPatternPhase.Active ||
            currentPhase == BossPatternPhase.Recovery;

        public virtual bool ShouldTrackTargetRotation => true;

        public virtual bool ShouldStopMotor => true;

        public float PhaseNormalizedProgress
        {
            get
            {
                float duration = GetCurrentPhaseDuration();

                if (duration <= 0f)
                {
                    return 0f;
                }

                return 1f - Mathf.Clamp01(remainingPhaseTime / duration);
            }
        }

        protected virtual void Awake()
        {
            context = GetComponent<EnemyContext>();
            combatController = GetComponent<BossCombatController>();

            if (!context.IsConfigured || !ValidatePatternConfiguration())
            {
                Debug.LogError($"[Boss] {GetType().Name}의 설정이 올바르지 않습니다.", this);

                return;
            }

            currentPhase = BossPatternPhase.Ready;
            isConfigured = true;

            OnPatternInitialized();
        }

        public bool CanBeSelected(Transform target, BossPhase bossPhase)
        {
            if (!isConfigured || !IsReady || !IsTargetAlive(target) || !IsAvailableInPhase(bossPhase))
            {
                return false;
            }

            return CanStartPattern(target);
        }

        public bool TryStartPattern(Transform target)
        {
            if (!CanBeSelected(target, combatController.CurrentPhase))
            {
                return false;
            }

            if (!combatController.TryBeginPattern(this))
            {
                return false;
            }

            currentTarget = target;
            currentPhase = BossPatternPhase.Windup;
            remainingPhaseTime = windup;

            OnWindupStarted();

            return true;
        }

        public void UpdatePattern(float deltaTime)
        {
            if (!isConfigured || currentPhase == BossPatternPhase.Ready || deltaTime <= 0f)
            {
                return;
            }

            if (IsActionLocked && !IsTargetAlive(currentTarget))
            {
                Cancel();
                return;
            }

            if (currentPhase == BossPatternPhase.Active)
            {
                OnActiveUpdated(deltaTime);

                if (currentPhase != BossPatternPhase.Active)
                {
                    return;
                }
            }

            remainingPhaseTime = Mathf.Max(0f, remainingPhaseTime - deltaTime);

            if (remainingPhaseTime > 0f)
            {
                return;
            }

            switch (currentPhase)
            {
                case BossPatternPhase.Windup:
                    StartActive();
                    break;

                case BossPatternPhase.Active:
                    StartRecovery();
                    break;

                case BossPatternPhase.Recovery:
                    StartCooldown();
                    break;

                case BossPatternPhase.Cooldown:
                    FinishCooldown();
                    break;
            }
        }

        public void Cancel()
        {
            if (currentPhase != BossPatternPhase.Ready &&
                combatController != null)
            {
                combatController.FinishPattern(this);
            }

            OnPatternCancelled();

            currentPhase = BossPatternPhase.Ready;
            remainingPhaseTime = 0f;
            currentTarget = null;
        }

        protected void CompleteActive()
        {
            if (currentPhase != BossPatternPhase.Active)
            {
                return;
            }

            StartRecovery();
        }

        protected DamageInfo CreateDamageInfo(float damageMultiplier)
        {
            return new DamageInfo(Context.Definition.Attack * damageMultiplier, gameObject);
        }

        protected bool TryApplyDamage(Transform target, float damageMultiplier)
        {
            if (!IsTargetAlive(target))
            {
                return false;
            }

            IDamageable damageable = target.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                return false;
            }

            damageable.TakeDamage(CreateDamageInfo(damageMultiplier));

            return true;
        }

        protected abstract bool CanStartPattern(Transform target);

        protected virtual bool IsAvailableInPhase(BossPhase bossPhase)
        {
            return true;
        }

        protected virtual bool ValidatePatternConfiguration()
        {
            return true;
        }

        protected virtual void OnPatternInitialized()
        {
        }

        protected virtual void OnWindupStarted()
        {
        }

        protected virtual void OnActiveStarted()
        {
        }

        protected virtual void OnActiveUpdated(float deltaTime)
        {
        }

        protected virtual void OnRecoveryStarted()
        {
        }

        protected virtual void OnCooldownStarted()
        {
        }

        protected virtual void OnPatternCancelled()
        {
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

        private void StartActive()
        {
            currentPhase = BossPatternPhase.Active;
            remainingPhaseTime = activeDuration;

            OnActiveStarted();
        }

        private void StartRecovery()
        {
            currentPhase = BossPatternPhase.Recovery;
            remainingPhaseTime = recovery;

            OnRecoveryStarted();
        }

        private void StartCooldown()
        {
            combatController.FinishPattern(this);

            currentPhase = BossPatternPhase.Cooldown;
            remainingPhaseTime = cooldown;
            currentTarget = null;

            OnCooldownStarted();
        }

        private void FinishCooldown()
        {
            currentPhase = BossPatternPhase.Ready;
            remainingPhaseTime = 0f;
        }

        private float GetCurrentPhaseDuration()
        {
            switch (currentPhase)
            {
                case BossPatternPhase.Windup:
                    return windup;

                case BossPatternPhase.Active:
                    return activeDuration;

                case BossPatternPhase.Recovery:
                    return recovery;

                case BossPatternPhase.Cooldown:
                    return cooldown;

                default:
                    return 0f;
            }
        }

        protected virtual void OnDisable()
        {
            if (currentPhase != BossPatternPhase.Ready)
            {
                Cancel();
            }
        }

        protected virtual void OnValidate()
        {
            windup = Mathf.Max(0f, windup);
            activeDuration = Mathf.Max(0.01f, activeDuration);
            recovery = Mathf.Max(0f, recovery);
            cooldown = Mathf.Max(0f, cooldown);
        }
    }
}