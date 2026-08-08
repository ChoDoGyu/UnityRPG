using UnityEngine;

namespace UnityRPG.Combat
{
    [RequireComponent(typeof(MeleeHitDetector))]
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
        [Min(0f)]
        private float attackDamage = 10f;

        [SerializeField]
        [Min(1)]
        private int maxComboCount = 3;

        private MeleeHitDetector hitDetector;

        private float remainingDuration;
        private int currentComboStep;
        private bool isNextAttackQueued;

        public bool IsAttacking =>
            remainingDuration > 0f;

        public bool CanAttack =>
            !IsAttacking;

        public int CurrentComboStep =>
            currentComboStep;

        private void Awake()
        {
            hitDetector = GetComponent<MeleeHitDetector>();
        }

        public void RequestAttack()
        {
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

            ApplyHit();
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
            if (attackReference == null)
            {
                Debug.LogError(
                    "[Combat] PlayerAttackController의 Attack Reference가 설정되지 않았습니다.");

                return;
            }

            var targets =
                hitDetector.FindTargets(
                    attackReference);

            DamageInfo damageInfo =
                new DamageInfo(
                    attackDamage,
                    gameObject);

            foreach (IDamageable target in targets)
            {
                target.TakeDamage(
                    damageInfo);
            }
        }
    }
}