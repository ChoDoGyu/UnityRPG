using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Core;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] MainMenuController의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            newGameButton.onClick.AddListener(HandleNewGame);
            quitButton.onClick.AddListener(HandleQuit);

            continueButton.interactable = false;
        }

        private void OnDestroy()
        {
            if (newGameButton != null)
                newGameButton.onClick.RemoveListener(HandleNewGame);

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

            SceneTransitionService.Instance.LoadScene(SceneNames.Hub);
        }

        private void HandleQuit()
        {
            Debug.Log("[UI] 게임 종료를 요청합니다.");
            Application.Quit();
        }

        private bool HasAllReferences()
        {
            return newGameButton != null &&
                   continueButton != null &&
                   quitButton != null;
        }
    }
}