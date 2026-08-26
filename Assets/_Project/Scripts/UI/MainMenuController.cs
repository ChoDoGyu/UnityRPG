using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Core;
using UnityRPG.Infrastructure.Save;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button quitButton;

        private SaveLoadCoordinator saveLoadCoordinator;

        private void Awake()
        {
            saveLoadCoordinator = FindFirstObjectByType<SaveLoadCoordinator>();
        }

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] MainMenuController의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            newGameButton.onClick.AddListener(HandleNewGame);
            continueButton.onClick.AddListener(HandleContinue);
            quitButton.onClick.AddListener(HandleQuit);

            RefreshContinueButton();
        }

        private void OnDestroy()
        {
            if (newGameButton != null)
                newGameButton.onClick.RemoveListener(HandleNewGame);

            if (continueButton != null)
                continueButton.onClick.RemoveListener(HandleContinue);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(HandleQuit);
        }

        private void HandleNewGame()
        {
            if (SceneTransitionService.Instance == null)
            {
                Debug.LogError("[UI] SceneTransitionService를 찾을 수 없습니다.", this);
                return;
            }

            if (!saveLoadCoordinator.ClearSave())
            {
                Debug.LogError("[Save] 기존 저장 데이터를 삭제하지 못했습니다.", this);
                return;
            }

            SceneTransitionService.Instance.LoadScene(SceneNames.Hub);
        }

        private void HandleContinue()
        {
            SaveLoadStatus status = saveLoadCoordinator.ContinueGame();

            if (status == SaveLoadStatus.Success)
                return;

            Debug.LogError($"[Save] 게임 불러오기를 시작하지 못했습니다: {status}", this);
            RefreshContinueButton();
        }

        private void HandleQuit()
        {
            Debug.Log("[UI] 게임 종료를 요청합니다.");
            Application.Quit();
        }

        private void RefreshContinueButton()
        {
            continueButton.interactable = saveLoadCoordinator.HasValidSave;
        }

        private bool HasAllReferences()
        {
            return newGameButton != null &&
                   continueButton != null &&
                   quitButton != null &&
                   saveLoadCoordinator != null;
        }
    }
}