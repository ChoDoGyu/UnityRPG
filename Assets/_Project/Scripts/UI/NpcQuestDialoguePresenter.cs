using UnityEngine;
using UnityRPG.Quest;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NpcQuestGiver))]
    public sealed class NpcQuestDialoguePresenter : MonoBehaviour
    {
        private NpcQuestGiver questGiver;

        private void Awake()
        {
            questGiver = GetComponent<NpcQuestGiver>();
        }

        private void OnEnable()
        {
            questGiver.DialogueRequested += HandleDialogueRequested;
        }

        private void OnDisable()
        {
            if (questGiver != null)
                questGiver.DialogueRequested -= HandleDialogueRequested;
        }

        private void HandleDialogueRequested(string speakerName, string dialogue)
        {
            NpcDialogueUI dialogueUI = FindFirstObjectByType<NpcDialogueUI>();

            if (dialogueUI == null)
            {
                Debug.LogError("[UI] NpcDialogueUI를 찾을 수 없습니다.", this);
                return;
            }

            dialogueUI.ShowDialogue(speakerName, dialogue);
        }
    }
}