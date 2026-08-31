using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class NpcDialogueUI : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private CanvasGroup dialogueGroup;
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text dialogueText;

        [Header("Display")]
        [SerializeField, Min(0.5f)] private float displayDuration = 4f;

        private float remainingTime;
        private bool isVisible;

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] NpcDialogueUI의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            SetVisible(false);
        }

        private void Update()
        {
            if (!isVisible)
                return;

            remainingTime -= Time.unscaledDeltaTime;

            if (remainingTime <= 0f)
                HideDialogue();
        }

        public void ShowDialogue(string speakerName, string dialogue)
        {
            if (string.IsNullOrWhiteSpace(dialogue))
                return;

            speakerNameText.text = speakerName;
            dialogueText.text = dialogue;
            remainingTime = displayDuration;

            SetVisible(true);
        }

        public void HideDialogue()
        {
            remainingTime = 0f;
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            isVisible = visible;
            dialogueGroup.alpha = visible ? 1f : 0f;
            dialogueGroup.interactable = false;
            dialogueGroup.blocksRaycasts = false;
        }

        private bool HasAllReferences()
        {
            return dialogueGroup != null &&
                   speakerNameText != null &&
                   dialogueText != null;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (dialogueGroup != null)
                HideDialogue();
        }
    }
}