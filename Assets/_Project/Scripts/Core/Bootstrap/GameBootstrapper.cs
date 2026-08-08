using System.Collections;
using UnityEngine;

namespace UnityRPG.Core
{
    [RequireComponent(typeof(SceneTransitionService))]
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private static GameBootstrapper instance;

        private SceneTransitionService sceneTransitionService;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

            sceneTransitionService =
                GetComponent<SceneTransitionService>();
        }

        private IEnumerator Start()
        {
            Debug.Log("[Bootstrap] 게임 초기화를 시작합니다.");

            yield return InitializeGame();

            Debug.Log("[Bootstrap] 게임 초기화가 완료되었습니다.");

            sceneTransitionService.LoadScene(SceneNames.MainMenu);
        }

        private IEnumerator InitializeGame()
        {
            yield return null;
        }
    }
}