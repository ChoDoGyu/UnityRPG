using UnityEngine;

namespace UnityRPG.Character.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerTargetFinder : MonoBehaviour
    {
        [Header("Search")]
        [SerializeField]
        [Min(0f)]
        private float searchRadius = 15f;

        [SerializeField]
        [Range(0f, 180f)]
        private float maxSearchAngle = 60f;

        [SerializeField]
        private LayerMask targetLayer;

        public LockOnTarget FindBestTarget(
            Transform cameraReference)
        {
            if (cameraReference == null)
            {
                return null;
            }

            Collider[] candidates =
                Physics.OverlapSphere(
                    transform.position,
                    searchRadius,
                    targetLayer,
                    QueryTriggerInteraction.Ignore);

            LockOnTarget bestTarget = null;

            float bestAngle =
                float.MaxValue;

            float bestSqrDistance =
                float.MaxValue;

            foreach (Collider candidate in candidates)
            {
                LockOnTarget target =
                    candidate.GetComponentInParent<LockOnTarget>();

                if (target == null)
                {
                    continue;
                }

                Vector3 toTarget =
                    target.AimPosition -
                    cameraReference.position;

                if (toTarget.sqrMagnitude <= 0.001f)
                {
                    continue;
                }

                float angle =
                    Vector3.Angle(
                        cameraReference.forward,
                        toTarget.normalized);

                if (angle > maxSearchAngle)
                {
                    continue;
                }

                float sqrDistance =
                    (target.transform.position -
                     transform.position).sqrMagnitude;

                bool hasBetterAngle =
                    angle < bestAngle;

                bool hasSameAngleButCloser =
                    Mathf.Approximately(
                        angle,
                        bestAngle) &&
                    sqrDistance < bestSqrDistance;

                if (!hasBetterAngle &&
                    !hasSameAngleButCloser)
                {
                    continue;
                }

                bestTarget =
                    target;

                bestAngle =
                    angle;

                bestSqrDistance =
                    sqrDistance;
            }

            return bestTarget;
        }
    }
}