using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private readonly HashSet<object> gameplayInputBlockers = new HashSet<object>();
        private readonly HashSet<object> uiInputBlockers = new HashSet<object>();

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

        public bool WasDeveloperConsolePressed => inputActions.Developer.ToggleConsole.WasPressedThisFrame();
        public bool WasInventoryPressed => inputActions.UI.Inventory.WasPressedThisFrame();
        public bool WasCancelPressed => inputActions.UI.Cancel.WasPressedThisFrame();

        private void Awake()
        {
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            inputActions.Developer.Enable();
            RefreshGameplayInputState();
            RefreshUIInputState();
        }

        private void OnDisable()
        {
            gameplayInputBlockers.Clear();
            uiInputBlockers.Clear();

            inputActions.Player.Disable();
            inputActions.UI.Disable();
            inputActions.Developer.Disable();
        }

        public void BlockGameplayInput(object source)
        {
            if (source == null)
                return;

            if (!gameplayInputBlockers.Add(source))
                return;

            RefreshGameplayInputState();
        }

        public void UnblockGameplayInput(object source)
        {
            if (source == null)
                return;

            if (!gameplayInputBlockers.Remove(source))
                return;

            RefreshGameplayInputState();
        }

        private void RefreshGameplayInputState()
        {
            bool shouldEnable = isActiveAndEnabled && gameplayInputBlockers.Count == 0;

            if (shouldEnable)
                inputActions.Player.Enable();
            else
                inputActions.Player.Disable();
        }

        public void BlockUIInput(object source)
        {
            if (source == null)
                return;

            if (!uiInputBlockers.Add(source))
                return;

            RefreshUIInputState();
        }

        public void UnblockUIInput(object source)
        {
            if (source == null)
                return;

            if (!uiInputBlockers.Remove(source))
                return;

            RefreshUIInputState();
        }

        private void RefreshUIInputState()
        {
            bool shouldEnable = isActiveAndEnabled && uiInputBlockers.Count == 0;

            if (shouldEnable)
                inputActions.UI.Enable();
            else
                inputActions.UI.Disable();
        }
    }
}