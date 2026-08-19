using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        private sealed class SpawnEntry
        {
            [SerializeField]
            private GameObject enemyPrefab;

            [SerializeField]
            private Transform spawnPoint;

            public GameObject EnemyPrefab => enemyPrefab;
            public Transform SpawnPoint => spawnPoint;
        }

        [Header("Spawn")]
        [SerializeField]
        private SpawnEntry[] spawnEntries = Array.Empty<SpawnEntry>();

        [Header("Runtime")]
        [SerializeField]
        private int aliveEnemyCount;

        [SerializeField]
        private bool hasSpawned;

        private readonly List<EnemyHealth> spawnedEnemies =
            new List<EnemyHealth>();

        public event Action AllEnemiesDefeated;

        public int AliveEnemyCount => aliveEnemyCount;
        public bool HasSpawned => hasSpawned;

        public bool AreAllEnemiesDefeated =>
            hasSpawned &&
            aliveEnemyCount <= 0;

        public bool SpawnAll()
        {
            if (hasSpawned)
            {
                return false;
            }

            if (!ValidateSpawnEntries())
            {
                return false;
            }

            hasSpawned = true;
            aliveEnemyCount = 0;
            spawnedEnemies.Clear();

            for (int i = 0; i < spawnEntries.Length; i++)
            {
                SpawnEntry entry = spawnEntries[i];

                GameObject enemyObject = Instantiate(
                    entry.EnemyPrefab,
                    entry.SpawnPoint.position,
                    entry.SpawnPoint.rotation);

                EnemyHealth enemyHealth =
                    enemyObject.GetComponent<EnemyHealth>();

                enemyHealth.Died += HandleEnemyDied;

                spawnedEnemies.Add(enemyHealth);
                aliveEnemyCount++;
            }

            return true;
        }

        private bool ValidateSpawnEntries()
        {
            if (spawnEntries == null || spawnEntries.Length == 0)
            {
                Debug.LogError(
                    "[Enemy] EnemySpawner에 Spawn Entry가 없습니다.",
                    this);

                return false;
            }

            bool isValid = true;

            for (int i = 0; i < spawnEntries.Length; i++)
            {
                SpawnEntry entry = spawnEntries[i];

                if (entry == null)
                {
                    Debug.LogError(
                        $"[Enemy] EnemySpawner의 Spawn Entry {i}가 비어 있습니다.",
                        this);

                    isValid = false;
                    continue;
                }

                if (entry.EnemyPrefab == null)
                {
                    Debug.LogError(
                        $"[Enemy] EnemySpawner의 Spawn Entry {i}에 Enemy Prefab이 없습니다.",
                        this);

                    isValid = false;
                }
                else if (entry.EnemyPrefab.GetComponent<EnemyHealth>() == null)
                {
                    Debug.LogError(
                        $"[Enemy] {entry.EnemyPrefab.name} Prefab에 EnemyHealth가 없습니다.",
                        entry.EnemyPrefab);

                    isValid = false;
                }

                if (entry.SpawnPoint == null)
                {
                    Debug.LogError(
                        $"[Enemy] EnemySpawner의 Spawn Entry {i}에 Spawn Point가 없습니다.",
                        this);

                    isValid = false;
                }
            }

            return isValid;
        }

        private void HandleEnemyDied()
        {
            if (aliveEnemyCount <= 0)
            {
                return;
            }

            aliveEnemyCount--;

            if (aliveEnemyCount == 0)
            {
                AllEnemiesDefeated?.Invoke();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                EnemyHealth enemyHealth = spawnedEnemies[i];

                if (enemyHealth != null)
                {
                    enemyHealth.Died -= HandleEnemyDied;
                }
            }
        }
    }
}