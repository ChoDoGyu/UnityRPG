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

        [SerializeField]
        [Min(0f)]
        private float dodgeCooldown = 0.5f;

        [Header("Invulnerability")]
        [SerializeField]
        [Min(0f)]
        private float invulnerabilityStart = 0.05f;

        [SerializeField]
        [Min(0f)]
        private float invulnerabilityEnd = 0.2f;

        private CharacterController characterController;

        private Vector3 dodgeDirection;
        private float remainingDuration;
        private float cooldownRemaining;
        private bool isConfigured;

        public bool IsDodging => remainingDuration > 0f;

        public Vector3 DodgeDirection => dodgeDirection;

        public bool CanDodge =>
            !IsDodging &&
            cooldownRemaining <= 0f;

        public float NormalizedProgress
        {
            get
            {
                if (!IsDodging || dodgeDuration <= 0f)
                {
                    return 0f;
                }

                return 1f - Mathf.Clamp01(remainingDuration / dodgeDuration);
            }
        }

        public bool IsInvulnerable
        {
            get
            {
                if (!IsDodging)
                {
                    return false;
                }

                float elapsedTime = dodgeDuration - remainingDuration;

                return elapsedTime >= invulnerabilityStart &&
                       elapsedTime <= invulnerabilityEnd;
            }
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (facingReference == null)
            {
                Debug.LogError(
                    "[Player] PlayerDodger의 Facing Reference가 설정되지 않았습니다.",
                    this);

                return;
            }

            isConfigured = true;
        }

        private void OnValidate()
        {
            invulnerabilityStart = Mathf.Clamp(
                invulnerabilityStart,
                0f,
                dodgeDuration);

            invulnerabilityEnd = Mathf.Clamp(
                invulnerabilityEnd,
                invulnerabilityStart,
                dodgeDuration);
        }

        public bool TryStartDodge(
            Vector2 moveInput,
            Transform cameraReference)
        {
            if (!isConfigured ||
                cameraReference == null ||
                !CanDodge)
            {
                return false;
            }

            dodgeDirection = CalculateDodgeDirection(
                moveInput,
                cameraReference);

            remainingDuration = dodgeDuration;
            cooldownRemaining = dodgeCooldown;

            return true;
        }

        public void UpdateDodge(float deltaTime)
        {
            if (!IsDodging)
            {
                return;
            }

            characterController.Move(
                dodgeDirection * dodgeSpeed * deltaTime);

            remainingDuration = Mathf.Max(
                0f,
                remainingDuration - deltaTime);
        }

        private Vector3 CalculateDodgeDirection(
            Vector2 moveInput,
            Transform cameraReference)
        {
            Vector2 clampedInput = Vector2.ClampMagnitude(
                moveInput,
                1f);

            Vector3 cameraForward = cameraReference.forward;
            Vector3 cameraRight = cameraReference.right;

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

            Vector3 facingDirection = facingReference.forward;
            facingDirection.y = 0f;

            return facingDirection.normalized;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (cooldownRemaining <= 0f)
            {
                return;
            }

            cooldownRemaining = Mathf.Max(
                0f,
                cooldownRemaining - deltaTime);
        }
    }
}