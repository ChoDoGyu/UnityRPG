using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.Core
{
    [DisallowMultipleComponent]
    public sealed class GameplayRootSceneBinder : MonoBehaviour
    {
        [SerializeField] private Transform playerRoot;

        private void Start()
        {
            if (playerRoot == null)
            {
                Debug.LogError("[GameplayRoot] PlayerRoot 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            ApplySpawnPoint();
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplySpawnPoint();
        }

        private void ApplySpawnPoint()
        {
            SceneSpawnPoint spawnPoint = FindFirstObjectByType<SceneSpawnPoint>();

            if (spawnPoint == null)
                return;

            spawnPoint.ApplyTo(playerRoot);
        }
    }
}