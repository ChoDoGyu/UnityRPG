using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyAttackController : MonoBehaviour
    {
        private EnemyContext context;
        private EnemyMeleeAttack meleeAttack;
        private EnemyRangedAttack rangedAttack;

        private float remainingCooldown;
        private bool isConfigured;

        public bool IsReady => remainingCooldown <= 0f;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
            meleeAttack = GetComponent<EnemyMeleeAttack>();
            rangedAttack = GetComponent<EnemyRangedAttack>();

            if (!context.IsConfigured)
            {
                return;
            }

            isConfigured = true;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (!isConfigured || IsReady || deltaTime <= 0f)
            {
                return;
            }

            remainingCooldown = Mathf.Max(0f, remainingCooldown - deltaTime);
        }

        public bool TryAttack(Transform target)
        {
            if (!isConfigured || !IsReady || target == null)
            {
                return false;
            }

            bool started;

            switch (context.Definition.EnemyType)
            {
                case EnemyType.Melee:
                    if (meleeAttack == null)
                    {
                        return false;
                    }

                    started = meleeAttack.TryAttack(target);
                    break;

                case EnemyType.Ranged:
                    if (rangedAttack == null)
                    {
                        return false;
                    }

                    started = rangedAttack.TryAttack(target);
                    break;

                default:
                    return false;
            }

            if (!started)
            {
                return false;
            }

            remainingCooldown = context.Definition.AttackCooldown;
            return true;
        }
    }
}