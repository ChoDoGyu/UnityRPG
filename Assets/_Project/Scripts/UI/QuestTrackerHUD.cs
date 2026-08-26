using System.Text;
using TMPro;
using UnityEngine;
using UnityRPG.Quest;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class QuestTrackerHUD : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerQuestLog playerQuestLog;

        [Header("View")]
        [SerializeField] private TMP_Text questNameText;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private TMP_Text statusText;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] QuestTrackerHUD의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            playerQuestLog.QuestAccepted += HandleQuestChanged;
            playerQuestLog.QuestProgressChanged += HandleQuestChanged;
            playerQuestLog.QuestReadyToTurnIn += HandleQuestChanged;
            playerQuestLog.QuestCompleted += HandleQuestChanged;
            playerQuestLog.QuestsRestored += HandleQuestsRestored;

            Refresh();
        }

        private void OnDestroy()
        {
            if (playerQuestLog == null)
                return;

            playerQuestLog.QuestAccepted -= HandleQuestChanged;
            playerQuestLog.QuestProgressChanged -= HandleQuestChanged;
            playerQuestLog.QuestReadyToTurnIn -= HandleQuestChanged;
            playerQuestLog.QuestCompleted -= HandleQuestChanged;
            playerQuestLog.QuestsRestored -= HandleQuestsRestored;
        }

        private void HandleQuestChanged(RuntimeQuest quest)
        {
            Refresh();
        }

        private void Refresh()
        {
            RuntimeQuest trackedQuest = FindTrackedQuest();

            if (trackedQuest == null)
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);

            questNameText.text = trackedQuest.Definition.DisplayName;
            objectiveText.text = BuildObjectiveText(trackedQuest);
            statusText.text = GetStatusText(trackedQuest.State);
        }

        private RuntimeQuest FindTrackedQuest()
        {
            for (int i = 0; i < playerQuestLog.Quests.Count; i++)
            {
                RuntimeQuest quest = playerQuestLog.Quests[i];

                if (quest.State != QuestState.Completed)
                    return quest;
            }

            return null;
        }

        private static string BuildObjectiveText(RuntimeQuest quest)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < quest.Objectives.Count; i++)
            {
                RuntimeQuestObjective objective = quest.Objectives[i];

                if (i > 0)
                    builder.AppendLine();

                builder.Append(GetObjectiveLabel(objective.Definition.Type));
                builder.Append("  ");
                builder.Append(objective.CurrentAmount);
                builder.Append(" / ");
                builder.Append(objective.Definition.RequiredAmount);
            }

            return builder.ToString();
        }

        private static string GetObjectiveLabel(QuestObjectiveType type)
        {
            switch (type)
            {
                case QuestObjectiveType.DefeatEnemy:
                    return "Defeat enemies";

                case QuestObjectiveType.CollectItem:
                    return "Collect items";

                default:
                    return "Objective";
            }
        }

        private static string GetStatusText(QuestState state)
        {
            switch (state)
            {
                case QuestState.Active:
                    return "In Progress";

                case QuestState.ReadyToTurnIn:
                    return "Ready to Turn In";

                default:
                    return string.Empty;
            }
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private bool HasAllReferences()
        {
            return playerQuestLog != null &&
                   questNameText != null &&
                   objectiveText != null &&
                   statusText != null;
        }

        private void HandleQuestsRestored()
        {
            Refresh();
        }
    }
}