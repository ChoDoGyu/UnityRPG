using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerController : MonoBehaviour
    {
        private CharacterController characterController;
        private PlayerInputReader inputReader;

        public CharacterController CharacterController =>
            characterController;

        public PlayerInputReader InputReader =>
            inputReader;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputReader = GetComponent<PlayerInputReader>();
        }


    }
}