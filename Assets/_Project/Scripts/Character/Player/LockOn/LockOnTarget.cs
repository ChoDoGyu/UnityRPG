using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class LockOnTarget : MonoBehaviour
    {
        [SerializeField]
        private Transform aimPoint;

        public Vector3 AimPosition =>
            aimPoint != null
                ? aimPoint.position
                : transform.position;
    }
}