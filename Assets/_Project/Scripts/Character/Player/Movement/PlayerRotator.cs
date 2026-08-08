using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerRotator : MonoBehaviour
    {
        [SerializeField]
        private Transform rotationTarget;

        [SerializeField]
        [Min(0f)]
        private float rotationSpeed = 720f;

        public void Rotate(
            Vector3 direction,
            float deltaTime)
        {
            if (rotationTarget == null)
            {
                return;
            }

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation =
                Quaternion.LookRotation(
                    direction,
                    Vector3.up);

            rotationTarget.rotation =
                Quaternion.RotateTowards(
                    rotationTarget.rotation,
                    targetRotation,
                    rotationSpeed * deltaTime);
        }

        public void SetFacingDirection(Vector3 direction)
        {
            if (rotationTarget == null ||
                direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            rotationTarget.rotation =
                Quaternion.LookRotation(
                    direction,
                    Vector3.up);
        }
    }
}