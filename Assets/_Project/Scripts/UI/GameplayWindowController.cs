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

        [Header("Pause")]
        [SerializeField] private GameObject pauseRoot;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button settingsBackButton;

        private InventoryUI inventoryUI;

        private bool isInventoryOpen;
        private bool isPauseOpen;
        private bool isSettingsOpen;
        private bool isGameplaySuspended;

        private float previousTimeScale = 1f;
        private bool previousCursorVisible;
        private CursorLockMode previousCursorLockState;

        public bool IsInventoryOpen => isInventoryOpen;
        public bool IsPauseOpen => isPauseOpen;
        public bool IsAnyWindowOpen => isInventoryOpen || isPauseOpen;

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
            resumeButton.onClick.AddListener(ClosePause);

            settingsButton.onClick.AddListener(OpenSettings);
            settingsBackButton.onClick.AddListener(CloseSettings);

            inventoryPanel.SetActive(false);
            pauseRoot.SetActive(false);

            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);

            isInventoryOpen = false;
            isPauseOpen = false;
        }

        private void Update()
        {
            if (inputReader.WasCancelPressed)
            {
                HandleCancel();
                return;
            }

            if (inputReader.WasInventoryPressed)
                HandleInventoryInput();
        }

        private void OnDisable()
        {
            if (isGameplaySuspended)
                ResumeGameplay();

            isInventoryOpen = false;
            isPauseOpen = false;
            isSettingsOpen = false;

            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);

            if (pauseRoot != null)
                pauseRoot.SetActive(false);

            if (pausePanel != null)
                pausePanel.SetActive(true);

            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (inventoryCloseButton != null)
                inventoryCloseButton.onClick.RemoveListener(CloseInventory);

            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(ClosePause);

            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OpenSettings);

            if (settingsBackButton != null)
                settingsBackButton.onClick.RemoveListener(CloseSettings);
        }

        private void HandleCancel()
        {
            if (isInventoryOpen)
            {
                CloseInventory();
                return;
            }

            if (isSettingsOpen)
            {
                CloseSettings();
                return;
            }

            if (isPauseOpen)
            {
                ClosePause();
                return;
            }

            OpenPause();
        }

        private void HandleInventoryInput()
        {
            if (isPauseOpen)
                return;

            if (isInventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        private void OpenInventory()
        {
            if (isInventoryOpen || isPauseOpen)
                return;

            isInventoryOpen = true;
            inventoryPanel.SetActive(true);

            RefreshGameplayState();
        }

        private void CloseInventory()
        {
            if (!isInventoryOpen)
                return;

            isInventoryOpen = false;

            inventoryUI.ResetView();
            inventoryPanel.SetActive(false);

            RefreshGameplayState();
        }

        private void OpenPause()
        {
            if (isPauseOpen || isInventoryOpen)
                return;

            isPauseOpen = true;
            isSettingsOpen = false;

            pauseRoot.SetActive(true);
            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);

            RefreshGameplayState();
        }

        private void ClosePause()
        {
            if (!isPauseOpen)
                return;

            isPauseOpen = false;
            isSettingsOpen = false;

            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);
            pauseRoot.SetActive(false);

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
                   inventoryCloseButton != null &&
                   pauseRoot != null &&
                   pausePanel != null &&
                   settingsPanel != null &&
                   resumeButton != null &&
                   settingsButton != null &&
                   settingsBackButton != null;
        }

        private void OpenSettings()
        {
            if (!isPauseOpen || isSettingsOpen)
                return;

            isSettingsOpen = true;

            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private void CloseSettings()
        {
            if (!isPauseOpen || !isSettingsOpen)
                return;

            isSettingsOpen = false;

            settingsPanel.SetActive(false);
            pausePanel.SetActive(true);
        }
    }
}