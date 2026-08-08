using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerRotator))]
    [RequireComponent(typeof(PlayerCameraController))]
    [RequireComponent(typeof(PlayerVisualAnimator))]
    [RequireComponent(typeof(PlayerStateController))]
    [RequireComponent(typeof(PlayerDodger))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerInputReader inputReader;
        private PlayerMotor playerMotor;
        private PlayerRotator playerRotator;
        private PlayerCameraController playerCameraController;
        private PlayerVisualAnimator visualAnimator;
        private PlayerStateController stateController;
        private PlayerDodger playerDodger;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            playerMotor = GetComponent<PlayerMotor>();
            playerRotator = GetComponent<PlayerRotator>();
            playerCameraController = GetComponent<PlayerCameraController>();
            visualAnimator = GetComponent<PlayerVisualAnimator>();
            stateController = GetComponent<PlayerStateController>();
            playerDodger = GetComponent<PlayerDodger>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            UpdateCamera(deltaTime);

            bool isDodgeFrame =
                UpdateDodge(deltaTime);

            if (!isDodgeFrame)
            {
                UpdateMovement(deltaTime);
            }

            UpdateVisual(deltaTime);
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

            if (stateController.CanRotate)
            {
                playerRotator.Rotate(
                    playerMotor.CurrentMoveDirection,
                    deltaTime);
            }
        }

        private void UpdateVisual(float deltaTime)
        {
            visualAnimator.UpdateAnimation(
                playerMotor.CurrentHorizontalSpeed,
                playerMotor.IsSprinting,
                deltaTime);
        }

        private bool UpdateDodge(float deltaTime)
        {
            if (inputReader.WasDodgePressed &&
                playerMotor.IsGrounded)
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
    }
}