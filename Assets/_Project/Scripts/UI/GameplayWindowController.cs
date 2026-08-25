using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Character.Player;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InventoryUI))]
    public sealed class GameplayWindowController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerInputReader inputReader;

        [Header("Inventory")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Button inventoryCloseButton;

        private InventoryUI inventoryUI;

        private bool isInventoryOpen;
        private bool isGameplaySuspended;

        private float previousTimeScale = 1f;
        private bool previousCursorVisible;
        private CursorLockMode previousCursorLockState;

        public bool IsInventoryOpen => isInventoryOpen;
        public bool IsAnyWindowOpen => isInventoryOpen;

        private void Awake()
        {
            inventoryUI = GetComponent<InventoryUI>();
        }

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] GameplayWindowController의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            inventoryCloseButton.onClick.AddListener(CloseInventory);
            SetInventoryOpen(false);
        }

        private void Update()
        {
            if (inputReader.WasInventoryPressed)
            {
                SetInventoryOpen(!isInventoryOpen);
                return;
            }

            if (isInventoryOpen && inputReader.WasCancelPressed)
                CloseInventory();
        }

        private void OnDisable()
        {
            if (isGameplaySuspended)
                ResumeGameplay();

            isInventoryOpen = false;

            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (inventoryCloseButton != null)
                inventoryCloseButton.onClick.RemoveListener(CloseInventory);
        }

        private void CloseInventory()
        {
            SetInventoryOpen(false);
        }

        private void SetInventoryOpen(bool open)
        {
            isInventoryOpen = open;

            if (!open)
                inventoryUI.ResetView();

            inventoryPanel.SetActive(open);
            RefreshGameplayState();
        }

        private void RefreshGameplayState()
        {
            if (IsAnyWindowOpen)
                SuspendGameplay();
            else
                ResumeGameplay();
        }

        private void SuspendGameplay()
        {
            if (isGameplaySuspended)
                return;

            previousTimeScale = Time.timeScale;
            previousCursorVisible = Cursor.visible;
            previousCursorLockState = Cursor.lockState;

            inputReader.BlockGameplayInput(this);

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            isGameplaySuspended = true;
        }

        private void ResumeGameplay()
        {
            if (!isGameplaySuspended)
                return;

            inputReader.UnblockGameplayInput(this);

            Time.timeScale = previousTimeScale;
            Cursor.lockState = previousCursorLockState;
            Cursor.visible = previousCursorVisible;

            isGameplaySuspended = false;
        }

        private bool HasAllReferences()
        {
            return inputReader != null &&
                   inventoryUI != null &&
                   inventoryPanel != null &&
                   inventoryCloseButton != null;
        }
    }
}