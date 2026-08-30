using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.Core
{
    public sealed class SceneTransitionService : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private GameObject loadingOverlay;
        [SerializeField] private CanvasGroup loadingCanvasGroup;

        [Header("Fade")]
        [SerializeField, Min(0.01f)] private float fadeDuration = 0.25f;

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

            if (loadingOverlay == null || loadingCanvasGroup == null)
                Debug.LogError("[Scene] LoadingOverlay 참조가 누락되었습니다.", this);

            if (loadingCanvasGroup != null)
                loadingCanvasGroup.alpha = 0f;

            if (loadingOverlay != null)
                loadingOverlay.SetActive(false);
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

            SetLoadingOverlay(true);
            yield return FadeLoadingOverlay(1f);

            Debug.Log($"[Scene] {targetScene} Scene으로 이동을 시작합니다.");

            AsyncOperation loadingSceneOperation = SceneManager.LoadSceneAsync(SceneNames.Loading);

            if (loadingSceneOperation == null)
            {
                Debug.LogError("[Scene] Loading Scene을 불러오지 못했습니다.");
                FinishTransition();
                yield break;
            }

            yield return loadingSceneOperation;
            yield return null;

            AsyncOperation targetSceneOperation = SceneManager.LoadSceneAsync(targetScene);

            if (targetSceneOperation == null)
            {
                Debug.LogError($"[Scene] {targetScene} Scene을 불러오지 못했습니다.");
                FinishTransition();
                yield break;
            }

            while (!targetSceneOperation.isDone)
            {
                LoadingProgress = Mathf.Clamp01(targetSceneOperation.progress / 0.9f);
                yield return null;
            }

            LoadingProgress = 1f;

            // Target Scene 활성화 후 Awake / Start / Spawn / Save Restore 등을
            // LoadingOverlay 뒤에서 마무리한다.
            yield return null;
            yield return new WaitForEndOfFrame();

            yield return FadeLoadingOverlay(0f);

            SetLoadingOverlay(false);
            IsTransitioning = false;

            Debug.Log($"[Scene] {targetScene} Scene 이동이 완료되었습니다.");
        }

        private IEnumerator FadeLoadingOverlay(float targetAlpha)
        {
            if (loadingCanvasGroup == null)
                yield break;

            float startAlpha = loadingCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);

                loadingCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            loadingCanvasGroup.alpha = targetAlpha;
        }

        private void FinishTransition()
        {
            if (loadingCanvasGroup != null)
                loadingCanvasGroup.alpha = 0f;

            SetLoadingOverlay(false);
            IsTransitioning = false;
        }

        private void SetLoadingOverlay(bool visible)
        {
            if (loadingOverlay != null)
                loadingOverlay.SetActive(visible);
        }
    }
}