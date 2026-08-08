using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerRotator))]
    [RequireComponent(typeof(PlayerCameraController))]
    public sealed class PlayerController : MonoBehaviour
    {
        private CharacterController characterController;
        private PlayerInputReader inputReader;
        private PlayerMotor playerMotor;
        private PlayerRotator playerRotator;
        private PlayerCameraController playerCameraController;

        public CharacterController CharacterController =>
            characterController;

        public PlayerInputReader InputReader =>
            inputReader;

        private void Awake()
        {
            characterController =
                GetComponent<CharacterController>();

            inputReader =
                GetComponent<PlayerInputReader>();

            playerMotor =
                GetComponent<PlayerMotor>();

            playerRotator =
                GetComponent<PlayerRotator>();

            playerCameraController =
                GetComponent<PlayerCameraController>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            playerCameraController.RotateCamera(
                inputReader.LookInput,
                inputReader.IsLookInputMouse,
                deltaTime);

            playerMotor.Move(
                inputReader.MoveInput,
                inputReader.IsSprintPressed,
                playerCameraController.CameraTarget,
                deltaTime);

            playerRotator.Rotate(
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }
    }
}