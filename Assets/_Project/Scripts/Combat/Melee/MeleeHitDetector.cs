using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Combat
{
    public sealed class MeleeHitDetector : MonoBehaviour
    {
        [Header("Hit Detection")]
        [SerializeField]
        [Min(0f)]
        private float hitRadius = 1f;

        [SerializeField]
        [Min(0f)]
        private float hitDistance = 1.5f;

        [SerializeField]
        private LayerMask targetLayer;

        public List<IDamageable> FindTargets(
            Transform attackReference)
        {
            List<IDamageable> targets =
                new List<IDamageable>();

            if (attackReference == null)
            {
                return targets;
            }

            Vector3 center =
                attackReference.position +
                attackReference.forward *
                hitDistance;

            Collider[] hits =
                Physics.OverlapSphere(
                    center,
                    hitRadius,
                    targetLayer,
                    QueryTriggerInteraction.Ignore);

            foreach (Collider hit in hits)
            {
                IDamageable damageable =
                    hit.GetComponentInParent<IDamageable>();

                if (damageable == null)
                {
                    continue;
                }

                if (targets.Contains(damageable))
                {
                    continue;
                }

                targets.Add(damageable);
            }

            return targets;
        }
    }
}