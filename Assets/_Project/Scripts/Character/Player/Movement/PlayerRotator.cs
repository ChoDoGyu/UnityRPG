using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerRotator : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        private float rotationSpeed = 720f;

        public void Rotate(Vector3 direction, float deltaTime)
        {
            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation =
                Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * deltaTime);
        }
    }
}