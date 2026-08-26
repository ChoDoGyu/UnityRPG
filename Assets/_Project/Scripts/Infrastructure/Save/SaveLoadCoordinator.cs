using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRPG.Core;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    public sealed class SaveLoadCoordinator : MonoBehaviour
    {
        private SaveFileService fileService;
        private string pendingSceneName;

        public bool HasSave => fileService != null && fileService.Exists();
        public bool HasValidSave => fileService != null && fileService.Load(out _) == SaveLoadStatus.Success;

        private void Awake()
        {
            fileService = new SaveFileService();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        public bool ClearSave()
        {
            return !fileService.Exists() || fileService.Delete();
        }

        public SaveLoadStatus ContinueGame()
        {
            if (SceneTransitionService.Instance == null)
                return SaveLoadStatus.RestoreFailed;

            SaveLoadStatus status = fileService.Load(out SaveGameData data);

            if (status != SaveLoadStatus.Success)
                return status;

            if (data.player == null || string.IsNullOrWhiteSpace(data.player.sceneName))
                return SaveLoadStatus.InvalidData;

            if (data.player.sceneName != SceneNames.Hub &&
                data.player.sceneName != SceneNames.Dungeon)
                return SaveLoadStatus.InvalidData;

            pendingSceneName = data.player.sceneName;
            SceneTransitionService.Instance.LoadScene(pendingSceneName);
            return SaveLoadStatus.Success;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (string.IsNullOrWhiteSpace(pendingSceneName) || scene.name != pendingSceneName)
                return;

            StartCoroutine(RestoreAfterSceneReady(scene.name));
        }

        private IEnumerator RestoreAfterSceneReady(string sceneName)
        {
            yield return null;

            if (pendingSceneName != sceneName)
                yield break;

            SaveGameController saveGameController = FindFirstObjectByType<SaveGameController>();

            if (saveGameController == null)
            {
                Debug.LogError("[Save] SaveGameController를 찾을 수 없습니다.", this);
                pendingSceneName = null;
                yield break;
            }

            SaveLoadStatus status = saveGameController.LoadGame();

            if (status != SaveLoadStatus.Success)
                Debug.LogError($"[Save] 게임 불러오기에 실패했습니다: {status}", this);

            pendingSceneName = null;
        }
    }
}