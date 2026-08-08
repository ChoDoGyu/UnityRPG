using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerRotator))]
    public sealed class PlayerController : MonoBehaviour
    {
        private CharacterController characterController;
        private PlayerInputReader inputReader;
        private PlayerMotor playerMotor;
        private PlayerRotator playerRotator;

        private Transform cameraTransform;

        public CharacterController CharacterController =>
            characterController;

        public PlayerInputReader InputReader =>
            inputReader;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputReader = GetComponent<PlayerInputReader>();
            playerMotor = GetComponent<PlayerMotor>();
            playerRotator = GetComponent<PlayerRotator>();
        }

        private void Start()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError(
                    "[Player] Main Camera를 찾을 수 없습니다.");

                enabled = false;
                return;
            }

            cameraTransform = mainCamera.transform;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            playerMotor.Move(
                inputReader.MoveInput,
                cameraTransform,
                deltaTime);

            playerRotator.Rotate(
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }
    }
}