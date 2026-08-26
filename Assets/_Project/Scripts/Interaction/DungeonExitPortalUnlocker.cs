using UnityEngine;
using UnityRPG.AI;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemySpawner))]
    public sealed class DungeonExitPortalUnlocker : MonoBehaviour
    {
        [SerializeField] private GameObject portalRoot;

        private EnemySpawner enemySpawner;

        private void Awake()
        {
            enemySpawner = GetComponent<EnemySpawner>();
        }

        private void Start()
        {
            if (portalRoot == null)
            {
                Debug.LogError("[Dungeon] 귀환 포탈 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            portalRoot.SetActive(enemySpawner.AreAllEnemiesDefeated);
        }

        private void OnEnable()
        {
            enemySpawner.AllEnemiesDefeated += HandleAllEnemiesDefeated;
        }

        private void OnDisable()
        {
            if (enemySpawner != null)
                enemySpawner.AllEnemiesDefeated -= HandleAllEnemiesDefeated;
        }

        private void HandleAllEnemiesDefeated()
        {
            portalRoot.SetActive(true);
        }
    }
}