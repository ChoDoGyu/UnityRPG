using UnityEngine;
using UnityRPG.Character.Player;
using UnityRPG.Combat;
using UnityRPG.VFX;
using UnityRPG.Core;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EliteSlamAttack : MonoBehaviour
    {
        [Header("Slam")]
        [SerializeField]
        [Min(0f)]
        private float range = 3f;

        [SerializeField]
        [Min(0f)]
        private float windup = 1f;

        [SerializeField]
        [Min(0f)]
        private float recovery = 0.8f;

        [SerializeField]
        [Min(0f)]
        private float cooldown = 5f;

        [SerializeField]
        [Min(0f)]
        private float damageMultiplier = 1.5f;

        [Header("VFX")]
        [SerializeField] private GameObject windupVfxPrefab;
        [SerializeField] private GameObject impactVfxPrefab;
        [SerializeField, Min(0f)] private float vfxGroundOffset = 0.05f;

        [Header("SFX")]
        [SerializeField] private AudioClip slamSfx;

        private GameObject activeWindupVfx;

        private EnemyContext context;

        private EnemyAttackPhase currentPhase = EnemyAttackPhase.Ready;
        private Transform currentTarget;
        private float remainingPhaseTime;

        public EnemyAttackPhase CurrentPhase => currentPhase;

        public float Range => range;

        public float DamageMultiplier => damageMultiplier;

        public bool IsReady => currentPhase == EnemyAttackPhase.Ready;

        public bool IsActionLocked => currentPhase == EnemyAttackPhase.Windup || currentPhase == EnemyAttackPhase.Recovery;

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

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
        }

        public bool TryStartSlam(Transform target)
        {
            if (!context.IsConfigured || !IsReady || !IsTargetAlive(target))
            {
                return false;
            }

            currentTarget = target;
            currentPhase = EnemyAttackPhase.Windup;
            remainingPhaseTime = windup;

            Vector3 position = transform.position + Vector3.up * vfxGroundOffset;
            activeWindupVfx = VfxSpawner.Spawn(windupVfxPrefab, position, Quaternion.identity);

            return true;
        }

        public void UpdateAttack(float deltaTime)
        {
            if (!context.IsConfigured || currentPhase == EnemyAttackPhase.Ready || deltaTime <= 0f)
            {
                return;
            }

            if (IsActionLocked && !IsTargetAlive(currentTarget))
            {
                Cancel();
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
                    ExecuteHitTiming();
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

        public void Cancel()
        {
            if (activeWindupVfx != null)
            {
                Destroy(activeWindupVfx);
                activeWindupVfx = null;
            }

            currentPhase = EnemyAttackPhase.Ready;
            remainingPhaseTime = 0f;
            currentTarget = null;
        }

        private void ExecuteHitTiming()
        {
            if (activeWindupVfx != null)
            {
                Destroy(activeWindupVfx);
                activeWindupVfx = null;
            }

            Vector3 impactPosition = transform.position + Vector3.up * vfxGroundOffset;

            VfxSpawner.Spawn(impactVfxPrefab, impactPosition, Quaternion.identity);
            AudioService.Instance?.PlaySfx(slamSfx);

            if (!IsTargetAlive(currentTarget))
                return;

            Vector3 direction = currentTarget.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > range * range)
                return;

            IDamageable damageable = currentTarget.GetComponentInParent<IDamageable>();

            if (damageable == null)
                return;

            DamageInfo damageInfo =
                new DamageInfo(context.Definition.Attack * damageMultiplier, gameObject);

            damageable.TakeDamage(damageInfo);
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

        private void StartRecovery()
        {
            currentPhase = EnemyAttackPhase.Recovery;
            remainingPhaseTime = recovery;
        }

        private void StartCooldown()
        {
            currentPhase = EnemyAttackPhase.Cooldown;
            remainingPhaseTime = cooldown;
            currentTarget = null;
        }

        private void FinishAttackCycle()
        {
            currentPhase = EnemyAttackPhase.Ready;
            remainingPhaseTime = 0f;
        }

        private float GetCurrentPhaseDuration()
        {
            switch (currentPhase)
            {
                case EnemyAttackPhase.Windup:
                    return windup;

                case EnemyAttackPhase.Recovery:
                    return recovery;

                case EnemyAttackPhase.Cooldown:
                    return cooldown;

                default:
                    return 0f;
            }
        }

        private void OnValidate()
        {
            range = Mathf.Max(0f, range);
            windup = Mathf.Max(0f, windup);
            recovery = Mathf.Max(0f, recovery);
            cooldown = Mathf.Max(0f, cooldown);
            damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }
    }
}