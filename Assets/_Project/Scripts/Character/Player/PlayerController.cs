using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerRotator))]
    [RequireComponent(typeof(PlayerCameraController))]
    [RequireComponent(typeof(PlayerVisualAnimator))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerInputReader inputReader;
        private PlayerMotor playerMotor;
        private PlayerRotator playerRotator;
        private PlayerCameraController playerCameraController;
        private PlayerVisualAnimator visualAnimator;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            playerMotor = GetComponent<PlayerMotor>();
            playerRotator = GetComponent<PlayerRotator>();
            playerCameraController = GetComponent<PlayerCameraController>();
            visualAnimator = GetComponent<PlayerVisualAnimator>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            UpdateCamera(deltaTime);
            UpdateMovement(deltaTime);
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

            playerRotator.Rotate(
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }

        private void UpdateVisual(float deltaTime)
        {
            visualAnimator.UpdateAnimation(
                playerMotor.CurrentHorizontalSpeed,
                playerMotor.IsSprinting,
                deltaTime);
        }
    }
}