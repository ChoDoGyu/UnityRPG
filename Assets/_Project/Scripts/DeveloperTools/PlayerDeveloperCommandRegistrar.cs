using UnityEngine;
using UnityRPG.Character.Growth;
using UnityRPG.Item;

namespace UnityRPG.DeveloperTools
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGrowth))]
    [RequireComponent(typeof(PlayerInventory))]
    public sealed class PlayerDeveloperCommandRegistrar : MonoBehaviour
    {
        [SerializeField] private DeveloperConsole developerConsole;
        [SerializeField] private ItemDatabase itemDatabase;

        private PlayerGrowth playerGrowth;
        private PlayerInventory inventory;

        private void Awake()
        {
            playerGrowth = GetComponent<PlayerGrowth>();
            inventory = GetComponent<PlayerInventory>();
        }

        private void Start()
        {
            if (developerConsole == null || itemDatabase == null || !developerConsole.IsAvailable)
                return;

            developerConsole.RegisterCommand(new ExpCommand(playerGrowth));
            developerConsole.RegisterCommand(new GiveItemCommand(inventory, itemDatabase));
        }
    }
}