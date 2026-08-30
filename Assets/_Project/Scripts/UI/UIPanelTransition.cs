using System.Collections;
using UnityEngine;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class UIPanelTransition : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panelTransform;

        [Header("Transition")]
        [SerializeField, Min(0.01f)] private float duration = 0.15f;
        [SerializeField, Range(0.8f, 1f)] private float hiddenScale = 0.96f;

        private Coroutine transitionRoutine;

        public void Show()
        {
            if (transitionRoutine != null)
                StopCoroutine(transitionRoutine);

            gameObject.SetActive(true);
            transitionRoutine = StartCoroutine(TransitionRoutine(true));
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            if (transitionRoutine != null)
                StopCoroutine(transitionRoutine);

            transitionRoutine = StartCoroutine(TransitionRoutine(false));
        }

        public void SetVisibleImmediate(bool visible)
        {
            if (transitionRoutine != null)
            {
                StopCoroutine(transitionRoutine);
                transitionRoutine = null;
            }

            gameObject.SetActive(visible);
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            panelTransform.localScale = visible ? Vector3.one : Vector3.one * hiddenScale;
        }

        private IEnumerator TransitionRoutine(bool show)
        {
            float startAlpha = canvasGroup.alpha;
            float targetAlpha = show ? 1f : 0f;
            Vector3 startScale = panelTransform.localScale;
            Vector3 targetScale = Vector3.one * (show ? 1f : hiddenScale);
            float elapsed = 0f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                panelTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            panelTransform.localScale = targetScale;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
            transitionRoutine = null;

            if (!show)
                gameObject.SetActive(false);
        }
    }
}