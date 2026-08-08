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

        private MeleeHitDetector hitDetector;
        private float remainingDuration;

        public bool IsAttacking =>
            remainingDuration > 0f;

        public bool CanAttack =>
            !IsAttacking;

        private void Awake()
        {
            hitDetector = GetComponent<MeleeHitDetector>();
        }

        public bool TryStartAttack()
        {
            if (!CanAttack)
            {
                return false;
            }

            remainingDuration =
                attackDuration;

            ApplyHit();

            return true;
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