using UnityEngine;
using UnityRPG.Character.Growth;

namespace UnityRPG.DeveloperTools
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGrowth))]
    public sealed class PlayerDeveloperCommandRegistrar : MonoBehaviour
    {
        [SerializeField] private DeveloperConsole developerConsole;

        private PlayerGrowth playerGrowth;

        private void Awake()
        {
            playerGrowth = GetComponent<PlayerGrowth>();
        }

        private void Start()
        {
            if (developerConsole == null || !developerConsole.IsAvailable)
                return;

            developerConsole.RegisterCommand(new ExpCommand(playerGrowth));
        }
    }
}