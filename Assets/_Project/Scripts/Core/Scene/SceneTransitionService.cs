using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.Core
{
    public sealed class SceneTransitionService : MonoBehaviour
    {
        public static SceneTransitionService Instance { get; private set; }

        public bool IsTransitioning { get; private set; }
        public float LoadingProgress { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void LoadScene(string targetScene)
        {
            if (IsTransitioning)
            {
                Debug.LogWarning($"[Scene] 이미 Scene 전환 중입니다. 요청을 무시합니다: {targetScene}");
                return;
            }

            if (targetScene == SceneNames.Bootstrap ||
                targetScene == SceneNames.Loading)
            {
                Debug.LogError($"[Scene] 이동 대상으로 사용할 수 없는 Scene입니다: {targetScene}");
                return;
            }

            StartCoroutine(LoadSceneRoutine(targetScene));
        }

        private IEnumerator LoadSceneRoutine(string targetScene)
        {
            IsTransitioning = true;
            LoadingProgress = 0f;

            Debug.Log($"[Scene] {targetScene} Scene으로 이동을 시작합니다.");

            // 모든 일반 Scene 이동은 먼저 Loading Scene을 거친다.
            AsyncOperation loadingSceneOperation = SceneManager.LoadSceneAsync(SceneNames.Loading);

            if (loadingSceneOperation == null)
            {
                Debug.LogError("[Scene] Loading Scene을 불러오지 못했습니다.");
                IsTransitioning = false;
                yield break;
            }

            yield return loadingSceneOperation;

            // Loading Scene이 최소 한 프레임은 실제로 표시되도록 한다.
            yield return null;

            AsyncOperation targetSceneOperation = SceneManager.LoadSceneAsync(targetScene);

            if (targetSceneOperation == null)
            {
                Debug.LogError($"[Scene] {targetScene} Scene을 불러오지 못했습니다.");

                IsTransitioning = false;
                yield break;
            }

            while (!targetSceneOperation.isDone)
            {
                LoadingProgress = Mathf.Clamp01(targetSceneOperation.progress / 0.9f);

                yield return null;
            }

            LoadingProgress = 1f;
            IsTransitioning = false;

            Debug.Log($"[Scene] {targetScene} Scene 이동이 완료되었습니다.");
        }
    }
}