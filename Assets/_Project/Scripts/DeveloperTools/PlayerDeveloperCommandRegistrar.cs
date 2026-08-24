using UnityEngine;
using UnityRPG.Character.Growth;
using UnityRPG.Item;
using UnityRPG.Quest;

namespace UnityRPG.DeveloperTools
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGrowth))]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(PlayerQuestLog))]
    public sealed class PlayerDeveloperCommandRegistrar : MonoBehaviour
    {
        [SerializeField] private DeveloperConsole developerConsole;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private QuestDatabase questDatabase;

        private PlayerGrowth playerGrowth;
        private PlayerInventory inventory;
        private PlayerQuestLog questLog;

        private void Awake()
        {
            playerGrowth = GetComponent<PlayerGrowth>();
            inventory = GetComponent<PlayerInventory>();
            questLog = GetComponent<PlayerQuestLog>();
        }

        private void Start()
        {
            if (developerConsole == null || !developerConsole.IsAvailable)
                return;

            developerConsole.RegisterCommand(new ExpCommand(playerGrowth));

            if (itemDatabase != null)
                developerConsole.RegisterCommand(new GiveItemCommand(inventory, itemDatabase));

            if (questDatabase != null)
                developerConsole.RegisterCommand(new QuestCommand(questLog, questDatabase));
        }
    }
}