using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    public sealed class EnemyMeleeAttack : MonoBehaviour
    {
        private EnemyContext context;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
        }

        public bool TryAttack(Transform target)
        {
            if (!context.IsConfigured || target == null)
            {
                return false;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float attackRange = context.Definition.AttackRange;

            if (direction.sqrMagnitude > attackRange * attackRange)
            {
                return false;
            }

            IDamageable damageable = target.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                return false;
            }

            DamageInfo damageInfo = new DamageInfo(
                context.Definition.Attack,
                gameObject);

            damageable.TakeDamage(damageInfo);

            return true;
        }
    }
}