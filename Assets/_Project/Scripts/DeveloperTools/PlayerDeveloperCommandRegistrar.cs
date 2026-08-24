using UnityEngine;
using UnityRPG.Character.Growth;
using UnityRPG.Infrastructure.Save;
using UnityRPG.Item;
using UnityRPG.Quest;

namespace UnityRPG.DeveloperTools
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DeveloperConsole))]
    [RequireComponent(typeof(PlayerGrowth))]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(PlayerQuestLog))]
    [RequireComponent(typeof(SaveGameController))]
    public sealed class PlayerDeveloperCommandRegistrar : MonoBehaviour
    {
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private QuestDatabase questDatabase;

        private DeveloperConsole developerConsole;
        private PlayerGrowth playerGrowth;
        private PlayerInventory inventory;
        private PlayerQuestLog questLog;
        private SaveGameController saveGameController;

        private void Awake()
        {
            developerConsole = GetComponent<DeveloperConsole>();
            playerGrowth = GetComponent<PlayerGrowth>();
            inventory = GetComponent<PlayerInventory>();
            questLog = GetComponent<PlayerQuestLog>();
            saveGameController = GetComponent<SaveGameController>();
        }

        private void Start()
        {
            if (!developerConsole.IsAvailable)
                return;

            developerConsole.RegisterCommand(new ExpCommand(playerGrowth));
            developerConsole.RegisterCommand(new SaveCommand(saveGameController));
            developerConsole.RegisterCommand(new LoadCommand(saveGameController));

            if (itemDatabase != null)
                developerConsole.RegisterCommand(new GiveItemCommand(inventory, itemDatabase));

            if (questDatabase != null)
                developerConsole.RegisterCommand(new QuestCommand(questLog, questDatabase));
        }
    }
}