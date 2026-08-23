using UnityEngine;
using UnityRPG.Character.Growth;
using UnityRPG.Interaction;
using UnityRPG.Item;

namespace UnityRPG.Quest
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NpcInteractable))]
    public sealed class NpcQuestGiver : MonoBehaviour
    {
        [SerializeField] private QuestDefinition questDefinition;

        private NpcInteractable npcInteractable;

        private void Awake()
        {
            npcInteractable = GetComponent<NpcInteractable>();
        }

        private void OnEnable()
        {
            if (npcInteractable != null)
                npcInteractable.Interacted += HandleInteracted;
        }

        private void OnDisable()
        {
            if (npcInteractable != null)
                npcInteractable.Interacted -= HandleInteracted;
        }

        private void HandleInteracted(GameObject interactor)
        {
            if (questDefinition == null || interactor == null)
                return;

            PlayerQuestLog questLog = interactor.GetComponentInParent<PlayerQuestLog>();

            if (questLog == null)
                return;

            RuntimeQuest quest = questLog.FindQuest(questDefinition.QuestId);

            if (quest == null)
            {
                questLog.TryAcceptQuest(questDefinition);
                return;
            }

            if (quest.State != QuestState.ReadyToTurnIn)
                return;

            PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
            PlayerGrowth playerGrowth = interactor.GetComponentInParent<PlayerGrowth>();

            if (inventory == null || playerGrowth == null)
                return;

            if (!TryGrantItemRewards(inventory))
                return;

            if (!questLog.TryCompleteQuest(questDefinition.QuestId))
            {
                RemoveItemRewards(inventory);
                return;
            }

            if (questDefinition.ExperienceReward > 0)
                playerGrowth.AddExperience(questDefinition.ExperienceReward);
        }

        private bool TryGrantItemRewards(PlayerInventory inventory)
        {
            for (int i = 0; i < questDefinition.ItemRewards.Count; i++)
            {
                QuestItemReward reward = questDefinition.ItemRewards[i];
                int added = inventory.AddItem(reward.Item, reward.Amount);

                if (added == reward.Amount)
                    continue;

                if (added > 0)
                    inventory.RemoveItem(reward.Item, added);

                for (int j = 0; j < i; j++)
                    inventory.RemoveItem(questDefinition.ItemRewards[j].Item, questDefinition.ItemRewards[j].Amount);

                return false;
            }

            return true;
        }

        private void RemoveItemRewards(PlayerInventory inventory)
        {
            for (int i = 0; i < questDefinition.ItemRewards.Count; i++)
            {
                QuestItemReward reward = questDefinition.ItemRewards[i];
                inventory.RemoveItem(reward.Item, reward.Amount);
            }
        }

        private void OnValidate()
        {
            if (questDefinition == null)
                Debug.LogWarning($"[Quest] {name}에 QuestDefinition이 설정되지 않았습니다.", this);
        }
    }
}