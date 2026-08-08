using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerDodger : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private Transform facingReference;

        [Header("Dodge")]
        [SerializeField]
        [Min(0.01f)]
        private float dodgeDuration = 0.25f;

        [SerializeField]
        [Min(0f)]
        private float dodgeSpeed = 12f;

        private CharacterController characterController;

        private Vector3 dodgeDirection;
        private float remainingDuration;
        private bool isConfigured;

        public bool IsDodging =>
            remainingDuration > 0f;

        public Vector3 DodgeDirection =>
            dodgeDirection;

        private void Awake()
        {
            characterController =
                GetComponent<CharacterController>();

            if (facingReference == null)
            {
                Debug.LogError(
                    "[Player] PlayerDodger의 Facing Reference가 설정되지 않았습니다.");

                return;
            }

            isConfigured = true;
        }

        public bool TryStartDodge(
            Vector2 moveInput,
            Transform cameraReference)
        {
            if (!isConfigured ||
                cameraReference == null)
            {
                return false;
            }

            dodgeDirection =
                CalculateDodgeDirection(
                    moveInput,
                    cameraReference);

            remainingDuration =
                dodgeDuration;

            return true;
        }

        public void UpdateDodge(float deltaTime)
        {
            if (!IsDodging)
            {
                return;
            }

            characterController.Move(
                dodgeDirection *
                dodgeSpeed *
                deltaTime);

            remainingDuration =
                Mathf.Max(
                    0f,
                    remainingDuration - deltaTime);
        }

        private Vector3 CalculateDodgeDirection(
            Vector2 moveInput,
            Transform cameraReference)
        {
            Vector2 clampedInput =
                Vector2.ClampMagnitude(
                    moveInput,
                    1f);

            Vector3 cameraForward =
                cameraReference.forward;

            Vector3 cameraRight =
                cameraReference.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 inputDirection =
                cameraForward * clampedInput.y +
                cameraRight * clampedInput.x;

            if (inputDirection.sqrMagnitude > 0.001f)
            {
                return inputDirection.normalized;
            }

            Vector3 facingDirection =
                facingReference.forward;

            facingDirection.y = 0f;

            return facingDirection.normalized;
        }
    }
}