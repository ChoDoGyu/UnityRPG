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
                    if (meleeAttack == null || !meleeAttack.isActiveAndEnabled)
                    {
                        return false;
                    }

                    started = meleeAttack.TryAttack(target);
                    break;

                case EnemyType.Ranged:
                    if (rangedAttack == null || !rangedAttack.isActiveAndEnabled)
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