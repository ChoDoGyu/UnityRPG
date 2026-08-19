using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(EnemySpawner))]
    public sealed class EncounterTrigger : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField]
        private bool hasStarted;

        [SerializeField]
        private bool isCompleted;

        private Collider triggerCollider;
        private EnemySpawner enemySpawner;
        private bool isConfigured;

        public bool HasStarted => hasStarted;
        public bool IsCompleted => isCompleted;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            enemySpawner = GetComponent<EnemySpawner>();

            if (!triggerCollider.isTrigger)
            {
                Debug.LogError(
                    "[Encounter] EncounterTrigger의 Collider는 Is Trigger가 활성화되어 있어야 합니다.",
                    this);

                return;
            }

            isConfigured = true;
        }

        private void OnEnable()
        {
            if (enemySpawner != null)
            {
                enemySpawner.AllEnemiesDefeated += HandleAllEnemiesDefeated;
            }
        }

        private void OnDisable()
        {
            if (enemySpawner != null)
            {
                enemySpawner.AllEnemiesDefeated -= HandleAllEnemiesDefeated;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isConfigured || hasStarted || isCompleted)
            {
                return;
            }

            PlayerController player =
                other.GetComponentInParent<PlayerController>();

            if (player == null)
            {
                return;
            }

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth == null || playerHealth.IsDead)
            {
                return;
            }

            if (!enemySpawner.SpawnAll())
            {
                return;
            }

            hasStarted = true;
        }

        private void HandleAllEnemiesDefeated()
        {
            if (!hasStarted || isCompleted)
            {
                return;
            }

            isCompleted = true;
        }
    }
}