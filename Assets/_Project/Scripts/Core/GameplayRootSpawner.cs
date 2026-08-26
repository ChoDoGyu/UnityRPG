using UnityEngine;

namespace UnityRPG.Core
{
    [DisallowMultipleComponent]
    public sealed class GameplayRootSpawner : MonoBehaviour
    {
        [SerializeField] private GameplayRootLifetime gameplayRootPrefab;

        private void Awake()
        {
            if (GameplayRootLifetime.HasInstance)
                return;

            if (gameplayRootPrefab == null)
            {
                Debug.LogError("[GameplayRoot] GameplayRoot Prefab 참조가 누락되었습니다.", this);
                return;
            }

            Instantiate(gameplayRootPrefab);
        }
    }
}