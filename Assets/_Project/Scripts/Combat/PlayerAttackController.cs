using UnityEngine;
using UnityRPG.Character.Stats;

namespace UnityRPG.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeleeHitDetector))]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerAttackController : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private Transform attackReference;

        [Header("Attack")]
        [SerializeField]
        [Min(0.01f)]
        private float attackDuration = 0.4f;

        [SerializeField]
        [Min(1)]
        private int maxComboCount = 3;

        [SerializeField]
        [Range(0f, 1f)]
        private float hitNormalizedTime = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float targetTrackingNormalizedTime = 0.25f;

        private MeleeHitDetector hitDetector;

        private float remainingDuration;
        private int currentComboStep;
        private bool isNextAttackQueued;
        private bool isHitApplied;
        private bool isConfigured;

        private PlayerStats playerStats;

        public bool IsAttacking =>
            remainingDuration > 0f;

        public int CurrentComboStep =>
            currentComboStep;

        public bool CanTrackTarget =>
            IsAttacking &&
            NormalizedProgress <= targetTrackingNormalizedTime;

        public float NormalizedProgress
        {
            get
            {
                if (currentComboStep <= 0)
                {
                    return 0f;
                }

                return Mathf.Clamp01(
                    1f -
                    remainingDuration / attackDuration);
            }
        }

        private void Awake()
        {
            hitDetector =
                GetComponent<MeleeHitDetector>();

            playerStats =
                GetComponent<PlayerStats>();

            if (attackReference == null)
            {
                Debug.LogError(
                    "[Combat] PlayerAttackController의 Attack Reference가 설정되지 않았습니다.",
                    this);

                return;
            }

            isConfigured = true;
        }

        public void RequestAttack()
        {
            if (!isConfigured)
            {
                return;
            }

            if (!IsAttacking)
            {
                StartAttack(1);
                return;
            }

            QueueNextAttack();
        }

        public void UpdateAttack(float deltaTime)
        {
            if (!IsAttacking)
            {
                return;
            }

            remainingDuration =
                Mathf.Max(
                    0f,
                    remainingDuration - deltaTime);

            float normalizedProgress =
                1f -
                remainingDuration / attackDuration;

            if (!isHitApplied &&
                normalizedProgress >= hitNormalizedTime)
            {
                ApplyHit();

                isHitApplied = true;
            }

            if (remainingDuration > 0f)
            {
                return;
            }

            if (isNextAttackQueued &&
                currentComboStep < maxComboCount)
            {
                StartAttack(
                    currentComboStep + 1);

                return;
            }

            EndCombo();
        }

        private void StartAttack(int comboStep)
        {
            currentComboStep =
                comboStep;

            remainingDuration =
                attackDuration;

            isNextAttackQueued =
                false;

            isHitApplied =
                false;
        }

        private void QueueNextAttack()
        {
            if (currentComboStep >= maxComboCount)
            {
                return;
            }

            isNextAttackQueued = true;
        }

        private void EndCombo()
        {
            currentComboStep = 0;
            isNextAttackQueued = false;
        }

        private void ApplyHit()
        {
            var targets =
                hitDetector.FindTargets(
                    attackReference);

            DamageInfo damageInfo =
                new DamageInfo(
                    playerStats.Attack,
                    gameObject);

            foreach (IDamageable target in targets)
            {
                target.TakeDamage(
                    damageInfo);
            }
        }
    }
}