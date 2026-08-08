using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private PlayerInputActions inputActions;

        public Vector2 MoveInput =>
            inputActions.Player.Move.ReadValue<Vector2>();

        public Vector2 LookInput =>
            inputActions.Player.Look.ReadValue<Vector2>();

        public bool IsSprintPressed =>
            inputActions.Player.Sprint.IsPressed();

        private void Awake()
        {
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            inputActions.Player.Disable();
        }
    }
}