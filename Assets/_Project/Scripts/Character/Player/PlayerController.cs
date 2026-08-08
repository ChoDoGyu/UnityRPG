using UnityEngine;
using UnityRPG.Combat;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerRotator))]
    [RequireComponent(typeof(PlayerCameraController))]
    [RequireComponent(typeof(PlayerVisualAnimator))]
    [RequireComponent(typeof(PlayerStateController))]
    [RequireComponent(typeof(PlayerDodger))]
    [RequireComponent(typeof(PlayerLockOnController))]
    [RequireComponent(typeof(PlayerAttackController))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerInputReader inputReader;
        private PlayerMotor playerMotor;
        private PlayerRotator playerRotator;
        private PlayerCameraController playerCameraController;
        private PlayerVisualAnimator visualAnimator;
        private PlayerStateController stateController;
        private PlayerDodger playerDodger;
        private PlayerLockOnController lockOnController;
        private PlayerAttackController attackController;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            playerMotor = GetComponent<PlayerMotor>();
            playerRotator = GetComponent<PlayerRotator>();
            playerCameraController = GetComponent<PlayerCameraController>();
            visualAnimator = GetComponent<PlayerVisualAnimator>();
            stateController = GetComponent<PlayerStateController>();
            playerDodger = GetComponent<PlayerDodger>();
            lockOnController = GetComponent<PlayerLockOnController>();
            attackController = GetComponent<PlayerAttackController>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            playerDodger.UpdateCooldown(deltaTime);
            attackController.UpdateAttack(deltaTime);

            lockOnController.ValidateCurrentTarget();

            UpdateLockOn();
            UpdateAttack();
            UpdateCamera(deltaTime);

            bool isDodging =
                UpdateDodge(deltaTime);

            if (!isDodging)
            {
                UpdateMovement(deltaTime);
            }

            UpdateVisual(
                isDodging,
                deltaTime);
        }

        private void UpdateCamera(float deltaTime)
        {
            playerCameraController.RotateCamera(
                inputReader.LookInput,
                inputReader.IsLookInputMouse,
                deltaTime);
        }

        private void UpdateMovement(float deltaTime)
        {
            if (!stateController.CanMove)
            {
                return;
            }

            Transform cameraTarget =
                playerCameraController.CameraTarget;

            if (cameraTarget == null)
            {
                return;
            }

            playerMotor.Move(
                inputReader.MoveInput,
                inputReader.IsSprintPressed,
                cameraTarget,
                deltaTime);

            UpdateRotation(deltaTime);
        }

        private void UpdateRotation(float deltaTime)
        {
            if (!stateController.CanRotate)
            {
                return;
            }

            if (lockOnController.IsLockedOn)
            {
                Vector3 targetDirection =
                    lockOnController.CurrentTarget.AimPosition -
                    transform.position;

                targetDirection.y = 0f;

                playerRotator.Rotate(
                    targetDirection,
                    deltaTime);

                return;
            }

            playerRotator.Rotate(
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }

        private void UpdateVisual(
            bool isDodging,
            float deltaTime)
        {
            visualAnimator.UpdateAnimation(
                playerMotor.CurrentHorizontalSpeed,
                playerMotor.IsSprinting,
                isDodging,
                lockOnController.IsLockedOn,
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }

        private bool UpdateDodge(float deltaTime)
        {
            if (inputReader.WasDodgePressed &&
                playerMotor.IsGrounded &&
                playerDodger.CanDodge)
            {
                if (stateController.TryEnterDodge())
                {
                    bool started =
                        playerDodger.TryStartDodge(
                            inputReader.MoveInput,
                            playerCameraController.CameraTarget);

                    if (!started)
                    {
                        stateController.ExitDodge();
                    }
                    else
                    {
                        playerRotator.SetFacingDirection(
                            playerDodger.DodgeDirection);
                    }
                }
            }

            if (stateController.CurrentState !=
                PlayerState.Dodging)
            {
                return false;
            }

            playerDodger.UpdateDodge(
                deltaTime);

            if (!playerDodger.IsDodging)
            {
                stateController.ExitDodge();
            }

            return true;
        }

        private void UpdateLockOn()
        {
            if (!inputReader.WasLockOnPressed)
            {
                return;
            }

            lockOnController.ToggleLockOn(
                playerCameraController.CameraTarget);
        }

        private void UpdateAttack()
        {
            if (!inputReader.WasAttackPressed)
            {
                return;
            }

            attackController.TryStartAttack();
        }
    }
}