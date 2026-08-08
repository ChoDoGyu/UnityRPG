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
    }
}