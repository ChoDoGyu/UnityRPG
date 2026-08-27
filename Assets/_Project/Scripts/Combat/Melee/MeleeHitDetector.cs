using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Combat
{
    [DisallowMultipleComponent]
    public sealed class MeleeHitDetector : MonoBehaviour
    {
        [Header("Hit Detection")]
        [SerializeField, Min(0f)] private float hitRadius = 1f;
        [SerializeField, Min(0f)] private float hitDistance = 1.5f;
        [SerializeField] private LayerMask targetLayer;

        public List<MeleeHitResult> FindHits(Transform attackReference)
        {
            List<MeleeHitResult> results = new();

            if (attackReference == null)
                return results;

            Vector3 center = attackReference.position + attackReference.forward * hitDistance;
            Collider[] hits = Physics.OverlapSphere(center, hitRadius, targetLayer, QueryTriggerInteraction.Ignore);
            Dictionary<IDamageable, Vector3> targets = new();

            foreach (Collider hit in hits)
            {
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                Vector3 point = hit.ClosestPoint(attackReference.position);

                if (!targets.TryGetValue(damageable, out Vector3 previousPoint) ||
                    (point - attackReference.position).sqrMagnitude < (previousPoint - attackReference.position).sqrMagnitude)
                    targets[damageable] = point;
            }

            foreach (KeyValuePair<IDamageable, Vector3> target in targets)
                results.Add(new MeleeHitResult(target.Key, target.Value));

            return results;
        }

        public List<IDamageable> FindTargets(Transform attackReference)
        {
            List<MeleeHitResult> hits = FindHits(attackReference);
            List<IDamageable> targets = new();

            for (int i = 0; i < hits.Count; i++)
                targets.Add(hits[i].Target);

            return targets;
        }
    }
}