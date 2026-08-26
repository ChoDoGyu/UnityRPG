using UnityEngine;
using UnityRPG.AI;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemySpawner))]
    public sealed class BossStatusSceneBinder : MonoBehaviour
    {
        [SerializeField] private string bossDisplayName = "Boss";

        private EnemySpawner enemySpawner;
        private BossStatusHUD bossStatusHUD;

        private void Awake()
        {
            enemySpawner = GetComponent<EnemySpawner>();
            bossStatusHUD = FindFirstObjectByType<BossStatusHUD>();
        }

        private void OnEnable()
        {
            enemySpawner.EnemySpawned += HandleEnemySpawned;
        }

        private void Start()
        {
            if (bossStatusHUD != null)
                return;

            Debug.LogError("[UI] BossStatusHUD를 찾을 수 없습니다.", this);
            enabled = false;
        }

        private void OnDisable()
        {
            if (enemySpawner != null)
                enemySpawner.EnemySpawned -= HandleEnemySpawned;

            if (bossStatusHUD != null)
                bossStatusHUD.Unbind();
        }

        private void HandleEnemySpawned(GameObject enemyObject)
        {
            if (!enemyObject.TryGetComponent(out BossController bossController))
                return;

            if (!enemyObject.TryGetComponent(out EnemyHealth bossHealth) ||
                !enemyObject.TryGetComponent(out BossCombatController bossCombatController))
            {
                Debug.LogError("[UI] Spawn된 Boss의 HUD 연결용 컴포넌트가 누락되었습니다.", enemyObject);
                return;
            }

            bossStatusHUD.Bind(bossHealth, bossCombatController, bossController, bossDisplayName);
        }
    }
}