using UnityEngine;

namespace UnityRPG.Combat
{
    public sealed class PlayerAttackController : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField]
        [Min(0.01f)]
        private float attackDuration = 0.4f;

        private float remainingDuration;

        public bool IsAttacking =>
            remainingDuration > 0f;

        public bool CanAttack =>
            !IsAttacking;

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