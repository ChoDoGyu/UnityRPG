using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        private const float MovingSpeedThreshold = 0.15f;

        [Header("Movement")]
        [SerializeField]
        [Min(0f)]
        private float moveSpeed = 5f;

        [SerializeField]
        [Min(0f)]
        private float sprintSpeed = 8f;

        [Header("Gravity")]
        [SerializeField]
        private float gravity = -20f;

        [SerializeField]
        private float groundedVerticalVelocity = -2f;

        private CharacterController characterController;
        private float verticalVelocity;

        public Vector3 CurrentMoveDirection { get; private set; }

        public float CurrentHorizontalSpeed { get; private set; }

        public bool IsGrounded =>
            characterController.isGrounded;

        public bool IsMoving =>
            CurrentHorizontalSpeed > MovingSpeedThreshold;

        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            characterController =
                GetComponent<CharacterController>();
        }

        public void Move(
            Vector2 input,
            bool sprintRequested,
            Transform cameraReference,
            float deltaTime)
        {
            UpdateHorizontalDirection(
                input,
                cameraReference);

            UpdateGravity(deltaTime);

            float currentSpeed =
                sprintRequested
                    ? sprintSpeed
                    : moveSpeed;

            Vector3 velocity =
                CurrentMoveDirection * currentSpeed;

            velocity.y = verticalVelocity;

            characterController.Move(
                velocity * deltaTime);

            UpdateMovementState(
                sprintRequested);
        }

        private void UpdateHorizontalDirection(
            Vector2 input,
            Transform cameraReference)
        {
            Vector2 clampedInput =
                Vector2.ClampMagnitude(input, 1f);

            Vector3 cameraForward =
                cameraReference.forward;

            Vector3 cameraRight =
                cameraReference.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            CurrentMoveDirection =
                cameraForward * clampedInput.y +
                cameraRight * clampedInput.x;

            if (CurrentMoveDirection.sqrMagnitude > 1f)
            {
                CurrentMoveDirection =
                    CurrentMoveDirection.normalized;
            }
        }

        private void UpdateGravity(float deltaTime)
        {
            if (IsGrounded &&
                verticalVelocity < 0f)
            {
                verticalVelocity =
                    groundedVerticalVelocity;

                return;
            }

            verticalVelocity +=
                gravity * deltaTime;
        }

        private void UpdateMovementState(
            bool sprintRequested)
        {
            Vector3 currentVelocity =
                characterController.velocity;

            currentVelocity.y = 0f;

            CurrentHorizontalSpeed =
                currentVelocity.magnitude;

            IsSprinting =
                sprintRequested &&
                IsMoving;
        }
    }
}