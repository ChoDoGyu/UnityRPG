using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Character.Player;
using UnityRPG.Core;
using UnityRPG.Infrastructure.Save;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InventoryUI))]
    public sealed class GameplayWindowController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private SaveGameController saveGameController;

        [Header("Inventory")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private UIPanelTransition inventoryTransition;
        [SerializeField] private Button inventoryCloseButton;

        [Header("Pause")]
        [SerializeField] private GameObject pauseRoot;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private UIPanelTransition pauseTransition;
        [SerializeField] private UIPanelTransition settingsTransition;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button settingsBackButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button saveButton;

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

            mainMenuButton.onClick.AddListener(HandleMainMenu);
            saveButton.onClick.AddListener(HandleSave);

            inventoryTransition.SetVisibleImmediate(false);

            pausePanel.SetActive(true);
            settingsTransition.SetVisibleImmediate(false);
            pauseTransition.SetVisibleImmediate(false);

            isInventoryOpen = false;
            isPauseOpen = false;
            isSettingsOpen = false;
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

            if (inventoryTransition != null)
                inventoryTransition.SetVisibleImmediate(false);

            if (settingsTransition != null)
                settingsTransition.SetVisibleImmediate(false);

            if (pausePanel != null)
                pausePanel.SetActive(true);

            if (pauseTransition != null)
                pauseTransition.SetVisibleImmediate(false);
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

            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(HandleMainMenu);

            if (saveButton != null)
                saveButton.onClick.RemoveListener(HandleSave);
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

            inventoryTransition.Show();
            UISfxService.Instance?.PlayOpen();

            RefreshGameplayState();
        }

        private void CloseInventory()
        {
            if (!isInventoryOpen)
                return;

            isInventoryOpen = false;

            inventoryUI.ResetView();
            inventoryTransition.Hide();
            UISfxService.Instance?.PlayClose();

            RefreshGameplayState();
        }

        private void OpenPause()
        {
            if (isPauseOpen || isInventoryOpen)
                return;

            isPauseOpen = true;
            isSettingsOpen = false;

            pausePanel.SetActive(true);
            settingsTransition.SetVisibleImmediate(false);

            pauseTransition.Show();
            UISfxService.Instance?.PlayOpen();

            RefreshGameplayState();
        }

        private void ClosePause()
        {
            if (!isPauseOpen)
                return;

            isPauseOpen = false;
            isSettingsOpen = false;

            settingsTransition.SetVisibleImmediate(false);
            pausePanel.SetActive(true);

            pauseTransition.Hide();
            UISfxService.Instance?.PlayClose();

            RefreshGameplayState();
        }

        private void OpenSettings()
        {
            if (!isPauseOpen || isSettingsOpen)
                return;

            isSettingsOpen = true;

            pausePanel.SetActive(false);
            settingsTransition.Show();

            UISfxService.Instance?.PlayOpen();
        }

        private void CloseSettings()
        {
            if (!isPauseOpen || !isSettingsOpen)
                return;

            isSettingsOpen = false;

            SettingsService.Instance?.SaveSettings();

            pausePanel.SetActive(true);
            settingsTransition.Hide();

            UISfxService.Instance?.PlayClose();
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

        private void HandleMainMenu()
        {
            if (SceneTransitionService.Instance == null)
            {
                Debug.LogError("[UI] SceneTransitionService를 찾을 수 없습니다.", this);
                return;
            }

            SaveLoadStatus status = saveGameController.SaveGame();

            if (status != SaveLoadStatus.Success)
            {
                Debug.LogError($"[Save] MainMenu 이동 전 저장에 실패했습니다: {status}", this);
                return;
            }

            ClosePause();
            GameplayRootLifetime.DestroyCurrent();
            SceneTransitionService.Instance.LoadScene(SceneNames.MainMenu);
        }

        private void HandleSave()
        {
            SaveLoadStatus status = saveGameController.SaveGame();

            if (status == SaveLoadStatus.Success)
            {
                UISfxService.Instance?.PlayClick();
                Debug.Log($"[Save] 게임을 저장했습니다: {saveGameController.SaveFilePath}", this);
                return;
            }

            Debug.LogError($"[Save] 게임 저장에 실패했습니다: {status}", this);
        }

        private bool HasAllReferences()
        {
            return inputReader != null &&
                   inventoryUI != null &&
                   inventoryPanel != null &&
                   inventoryTransition != null &&
                   inventoryCloseButton != null &&
                   pauseRoot != null &&
                   pausePanel != null &&
                   settingsPanel != null &&
                   pauseTransition != null &&
                   settingsTransition != null &&
                   resumeButton != null &&
                   settingsButton != null &&
                   settingsBackButton != null &&
                   saveGameController != null &&
                   saveButton != null &&
                   mainMenuButton != null;
        }
    }
}