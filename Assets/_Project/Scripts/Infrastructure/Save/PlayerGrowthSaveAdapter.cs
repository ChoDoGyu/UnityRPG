using UnityEngine;
using UnityRPG.Character.Growth;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGrowth))]
    public sealed class PlayerGrowthSaveAdapter : MonoBehaviour
    {
        private PlayerGrowth playerGrowth;

        private void Awake()
        {
            playerGrowth = GetComponent<PlayerGrowth>();
        }

        public bool Capture(PlayerSaveData data)
        {
            if (data == null || !playerGrowth.IsConfigured)
                return false;

            data.level = playerGrowth.CurrentLevel;
            data.currentExperience = playerGrowth.CurrentExperience;
            return true;
        }

        public bool Restore(PlayerSaveData data)
        {
            if (data == null || !playerGrowth.IsConfigured)
                return false;

            return playerGrowth.TryRestoreProgress(data.level, data.currentExperience);
        }
    }
}