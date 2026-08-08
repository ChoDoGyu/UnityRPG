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

        private void Awake()
        {
            if (rotationTarget == null)
            {
                Debug.LogError(
                    "[Player] PlayerRotator의 Rotation Target이 설정되지 않았습니다.");
            }
        }

        public void Rotate(Vector3 direction, float deltaTime)
        {
            if (rotationTarget == null ||
                direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation =
                Quaternion.LookRotation(direction, Vector3.up);

            rotationTarget.rotation =
                Quaternion.RotateTowards(
                    rotationTarget.rotation,
                    targetRotation,
                    rotationSpeed * deltaTime);
        }
    }
}