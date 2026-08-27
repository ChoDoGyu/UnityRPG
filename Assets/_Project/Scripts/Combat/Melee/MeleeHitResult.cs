using UnityEngine;

namespace UnityRPG.Combat
{
    public readonly struct MeleeHitResult
    {
        public IDamageable Target { get; }
        public Vector3 Point { get; }

        public MeleeHitResult(IDamageable target, Vector3 point)
        {
            Target = target;
            Point = point;
        }
    }
}