using UnityEngine;
using UnityRPG.Character.Player;
using System;

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

        [Header("Identity")]
        [SerializeField] private string encounterId;

        private Collider triggerCollider;
        private EnemySpawner enemySpawner;
        private bool isConfigured;

        public bool HasStarted => hasStarted;
        public bool IsCompleted => isCompleted;
        public string EncounterId => encounterId;

        public event Action StateChanged;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            enemySpawner = GetComponent<EnemySpawner>();

            if (!triggerCollider.isTrigger)
            {
                Debug.LogError("[Encounter] EncounterTrigger의 Collider는 Is Trigger가 활성화되어 있어야 합니다.", this);

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

            PlayerController player = other.GetComponentInParent<PlayerController>();

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
            StateChanged?.Invoke();
        }

        private void HandleAllEnemiesDefeated()
        {
            if (!hasStarted || isCompleted)
                return;

            isCompleted = true;
            triggerCollider.enabled = false;
            StateChanged?.Invoke();
        }

        public bool RestoreState(bool started, bool completed)
        {
            if (!isConfigured || string.IsNullOrWhiteSpace(encounterId))
                return false;

            if (completed && !started)
                return false;

            if (completed)
            {
                hasStarted = true;
                isCompleted = true;
                triggerCollider.enabled = false;
                StateChanged?.Invoke();
                return true;
            }

            if (started)
            {
                if (!enemySpawner.SpawnAll())
                    return false;

                hasStarted = true;
                isCompleted = false;
                triggerCollider.enabled = false;
                StateChanged?.Invoke();
                return true;
            }

            hasStarted = false;
            isCompleted = false;
            triggerCollider.enabled = true;
            StateChanged?.Invoke();
            return true;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(encounterId))
                Debug.LogWarning($"[Encounter] {name}의 EncounterId가 비어 있습니다.", this);
        }
    }
}