using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGrowthSaveAdapter))]
    [RequireComponent(typeof(PlayerItemSaveAdapter))]
    [RequireComponent(typeof(PlayerQuestSaveAdapter))]
    [RequireComponent(typeof(PlayerTransformSaveAdapter))]
    [RequireComponent(typeof(PlayerCheckpointSaveAdapter))]
    [RequireComponent(typeof(EncounterSaveAdapter))]
    public sealed class SaveGameController : MonoBehaviour
    {
        private SaveFileService fileService;
        private PlayerGrowthSaveAdapter growthAdapter;
        private PlayerItemSaveAdapter itemAdapter;
        private PlayerQuestSaveAdapter questAdapter;
        private PlayerTransformSaveAdapter transformAdapter;
        private PlayerCheckpointSaveAdapter checkpointAdapter;
        private EncounterSaveAdapter encounterAdapter;

        public string SaveFilePath => fileService.FilePath;

        private void Awake()
        {
            fileService = new SaveFileService();
            growthAdapter = GetComponent<PlayerGrowthSaveAdapter>();
            itemAdapter = GetComponent<PlayerItemSaveAdapter>();
            questAdapter = GetComponent<PlayerQuestSaveAdapter>();
            transformAdapter = GetComponent<PlayerTransformSaveAdapter>();
            checkpointAdapter = GetComponent<PlayerCheckpointSaveAdapter>();
            encounterAdapter = GetComponent<EncounterSaveAdapter>();
        }

        public SaveLoadStatus SaveGame()
        {
            SaveGameData data = new();

            if (!growthAdapter.Capture(data.player))
                return SaveLoadStatus.CaptureFailed;

            if (!itemAdapter.Capture(data))
                return SaveLoadStatus.CaptureFailed;

            if (!questAdapter.Capture(data))
                return SaveLoadStatus.CaptureFailed;

            if (!encounterAdapter.Capture(data))
                return SaveLoadStatus.CaptureFailed;

            if (!transformAdapter.Capture(data.player))
                return SaveLoadStatus.CaptureFailed;

            if (!checkpointAdapter.Capture(data.checkpoint))
                return SaveLoadStatus.CaptureFailed;

            return fileService.Save(data);
        }

        public SaveLoadStatus LoadGame()
        {
            SaveLoadStatus loadStatus = fileService.Load(out SaveGameData data);

            if (loadStatus != SaveLoadStatus.Success)
                return loadStatus;

            if (data.player.sceneName != SceneManager.GetActiveScene().name)
                return SaveLoadStatus.SceneMismatch;

            if (!growthAdapter.Restore(data.player))
                return SaveLoadStatus.RestoreFailed;

            if (!itemAdapter.Restore(data))
                return SaveLoadStatus.RestoreFailed;

            if (!questAdapter.Restore(data))
                return SaveLoadStatus.RestoreFailed;

            if (!encounterAdapter.Restore(data))
                return SaveLoadStatus.RestoreFailed;

            if (!checkpointAdapter.Restore(data.checkpoint))
                return SaveLoadStatus.RestoreFailed;

            if (!transformAdapter.Restore(data.player))
                return SaveLoadStatus.RestoreFailed;

            return SaveLoadStatus.Success;
        }
    }
}