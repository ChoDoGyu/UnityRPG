using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        private float moveSpeed = 5f;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void Move(Vector2 input, Transform cameraTransform, float deltaTime)
        {
            Vector2 clampedInput = Vector2.ClampMagnitude(input, 1f);

            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection =
                cameraForward * clampedInput.y +
                cameraRight * clampedInput.x;

            if (moveDirection.sqrMagnitude > 1f)
            {
                moveDirection.Normalize();
            }

            characterController.Move(
                moveDirection * moveSpeed * deltaTime);
        }
    }
}