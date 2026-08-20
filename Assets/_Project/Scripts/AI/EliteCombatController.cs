using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyAttackController))]
    [RequireComponent(typeof(EliteSlamAttack))]
    public sealed class EliteCombatController : MonoBehaviour
    {
        private EnemyContext context;
        private EnemyAttackController normalAttack;
        private EliteSlamAttack slamAttack;

        private bool isConfigured;

        public bool IsActionLocked =>
            isConfigured &&
            (normalAttack.IsActionLocked ||
             slamAttack.IsActionLocked);

        public bool IsSlamActionLocked =>
            isConfigured &&
            slamAttack.IsActionLocked;

        public EnemyAttackPhase CurrentPhase
        {
            get
            {
                if (!isConfigured)
                {
                    return EnemyAttackPhase.Ready;
                }

                if (slamAttack.IsActionLocked)
                {
                    return slamAttack.CurrentPhase;
                }

                if (normalAttack.IsActionLocked)
                {
                    return normalAttack.CurrentPhase;
                }

                if (normalAttack.CurrentPhase != EnemyAttackPhase.Ready)
                {
                    return normalAttack.CurrentPhase;
                }

                return slamAttack.CurrentPhase;
            }
        }

        public float PhaseNormalizedProgress
        {
            get
            {
                if (!isConfigured)
                {
                    return 0f;
                }

                if (slamAttack.IsActionLocked)
                {
                    return slamAttack.PhaseNormalizedProgress;
                }

                if (normalAttack.IsActionLocked)
                {
                    return normalAttack.PhaseNormalizedProgress;
                }

                if (normalAttack.CurrentPhase != EnemyAttackPhase.Ready)
                {
                    return normalAttack.PhaseNormalizedProgress;
                }

                return slamAttack.PhaseNormalizedProgress;
            }
        }

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
            normalAttack = GetComponent<EnemyAttackController>();
            slamAttack = GetComponent<EliteSlamAttack>();

            if (!context.IsConfigured)
            {
                return;
            }

            isConfigured = true;
        }

        public void UpdateCombat(float deltaTime)
        {
            if (!isConfigured || deltaTime <= 0f)
            {
                return;
            }

            normalAttack.UpdateAttack(deltaTime);
            slamAttack.UpdateAttack(deltaTime);
        }

        public bool ShouldEnterAttackState(Transform target)
        {
            if (!isConfigured || target == null)
            {
                return false;
            }

            if (IsActionLocked)
            {
                return true;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float sqrDistance = direction.sqrMagnitude;
            float normalRange = context.Definition.AttackRange;

            if (sqrDistance <= normalRange * normalRange)
            {
                return true;
            }

            return slamAttack.IsReady &&
                   sqrDistance <= slamAttack.Range * slamAttack.Range;
        }

        public bool TryStartAttack(Transform target)
        {
            if (!isConfigured ||
                target == null ||
                IsActionLocked)
            {
                return false;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float sqrDistance = direction.sqrMagnitude;

            if (slamAttack.IsReady &&
                sqrDistance <= slamAttack.Range * slamAttack.Range)
            {
                return slamAttack.TryStartSlam(target);
            }

            float normalRange = context.Definition.AttackRange;

            if (normalAttack.IsReady &&
                sqrDistance <= normalRange * normalRange)
            {
                return normalAttack.TryStartAttack(target);
            }

            return false;
        }
    }
}