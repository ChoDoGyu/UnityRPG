using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private PlayerInputActions inputActions;

        public Vector2 MoveInput => inputActions.Player.Move.ReadValue<Vector2>();

        public Vector2 LookInput => inputActions.Player.Look.ReadValue<Vector2>();

        public bool IsSprintPressed => inputActions.Player.Sprint.IsPressed();

        public bool IsLookInputMouse => inputActions.Player.Look.activeControl?.device is Mouse;

        public bool WasDodgePressed => inputActions.Player.Dodge.WasPressedThisFrame();

        public bool WasLockOnPressed => inputActions.Player.LockOn.WasPressedThisFrame();

        public bool WasAttackPressed => inputActions.Player.Attack.WasPressedThisFrame();

        public bool WasInteractPressed => inputActions.Player.Interact.WasPressedThisFrame();

        public bool WasSkill1Pressed => inputActions.Player.Skill1.WasPressedThisFrame();

        public bool WasSkill2Pressed => inputActions.Player.Skill2.WasPressedThisFrame();

        public bool WasSkill3Pressed => inputActions.Player.Skill3.WasPressedThisFrame();

        public bool WasSkill4Pressed => inputActions.Player.Skill4.WasPressedThisFrame();

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