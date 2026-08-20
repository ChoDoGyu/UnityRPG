using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        private sealed class SpawnGroup
        {
            [SerializeField] private GameObject enemyPrefab;
            [SerializeField, Min(1)] private int count = 1;

            public GameObject EnemyPrefab => enemyPrefab;
            public int Count => count;
        }

        [Header("Spawn")]
        [SerializeField] private SpawnGroup[] spawnGroups = Array.Empty<SpawnGroup>();
        [SerializeField] private BoxCollider spawnArea;
        [SerializeField, Min(0.1f)] private float minimumSpacing = 1.5f;
        [SerializeField, Min(0.1f)] private float navMeshSampleDistance = 1.5f;
        [SerializeField, Min(1)] private int maxPositionAttempts = 30;

        [Header("Runtime")]
        [SerializeField] private int aliveEnemyCount;
        [SerializeField] private bool hasSpawned;

        private readonly List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();

        public event Action AllEnemiesDefeated;

        public int AliveEnemyCount => aliveEnemyCount;
        public bool HasSpawned => hasSpawned;
        public bool AreAllEnemiesDefeated => hasSpawned && aliveEnemyCount <= 0;

        public bool SpawnAll()
        {
            if (hasSpawned)
                return false;

            if (!ValidateConfiguration())
                return false;

            List<GameObject> prefabs = new List<GameObject>();
            List<Vector3> positions = new List<Vector3>();

            if (!TryBuildSpawnPlan(prefabs, positions))
                return false;

            hasSpawned = true;
            aliveEnemyCount = 0;
            spawnedEnemies.Clear();

            for (int i = 0; i < prefabs.Count; i++)
            {
                GameObject enemyObject = Instantiate(prefabs[i], positions[i], Quaternion.identity);
                EnemyHealth enemyHealth = enemyObject.GetComponent<EnemyHealth>();

                enemyHealth.Died += HandleEnemyDied;
                spawnedEnemies.Add(enemyHealth);
                aliveEnemyCount++;
            }

            return true;
        }

        private bool TryBuildSpawnPlan(List<GameObject> prefabs, List<Vector3> positions)
        {
            for (int i = 0; i < spawnGroups.Length; i++)
            {
                SpawnGroup group = spawnGroups[i];

                for (int j = 0; j < group.Count; j++)
                {
                    if (!TryFindSpawnPosition(positions, out Vector3 position))
                    {
                        Debug.LogError($"[Enemy] {group.EnemyPrefab.name}의 Spawn 위치를 찾지 못했습니다.", this);
                        return false;
                    }

                    prefabs.Add(group.EnemyPrefab);
                    positions.Add(position);
                }
            }

            return true;
        }

        private bool TryFindSpawnPosition(List<Vector3> usedPositions, out Vector3 position)
        {
            Vector3 center = spawnArea.center;
            Vector3 halfSize = spawnArea.size * 0.5f;

            for (int i = 0; i < maxPositionAttempts; i++)
            {
                Vector3 localPosition = new Vector3(
                    UnityEngine.Random.Range(center.x - halfSize.x, center.x + halfSize.x),
                    center.y,
                    UnityEngine.Random.Range(center.z - halfSize.z, center.z + halfSize.z));

                Vector3 worldPosition = spawnArea.transform.TransformPoint(localPosition);

                if (!NavMesh.SamplePosition(worldPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
                    continue;

                if (!IsInsideSpawnArea(hit.position))
                    continue;

                if (!HasEnoughSpacing(hit.position, usedPositions))
                    continue;

                position = hit.position;
                return true;
            }

            position = Vector3.zero;
            return false;
        }

        private bool IsInsideSpawnArea(Vector3 worldPosition)
        {
            Vector3 localPosition = spawnArea.transform.InverseTransformPoint(worldPosition);
            Vector3 center = spawnArea.center;
            Vector3 halfSize = spawnArea.size * 0.5f;

            return localPosition.x >= center.x - halfSize.x &&
                   localPosition.x <= center.x + halfSize.x &&
                   localPosition.z >= center.z - halfSize.z &&
                   localPosition.z <= center.z + halfSize.z;
        }

        private bool HasEnoughSpacing(Vector3 position, List<Vector3> usedPositions)
        {
            float minimumSpacingSqr = minimumSpacing * minimumSpacing;

            for (int i = 0; i < usedPositions.Count; i++)
            {
                Vector3 difference = position - usedPositions[i];
                difference.y = 0f;

                if (difference.sqrMagnitude < minimumSpacingSqr)
                    return false;
            }

            return true;
        }

        private bool ValidateConfiguration()
        {
            if (spawnGroups == null || spawnGroups.Length == 0)
            {
                Debug.LogError("[Enemy] EnemySpawner에 Spawn Group이 없습니다.", this);
                return false;
            }

            if (spawnArea == null)
            {
                Debug.LogError("[Enemy] EnemySpawner에 Spawn Area가 없습니다.", this);
                return false;
            }

            bool isValid = true;

            for (int i = 0; i < spawnGroups.Length; i++)
            {
                SpawnGroup group = spawnGroups[i];

                if (group == null)
                {
                    Debug.LogError($"[Enemy] Spawn Group {i}가 비어 있습니다.", this);
                    isValid = false;
                    continue;
                }

                if (group.EnemyPrefab == null)
                {
                    Debug.LogError($"[Enemy] Spawn Group {i}에 Enemy Prefab이 없습니다.", this);
                    isValid = false;
                }
                else if (group.EnemyPrefab.GetComponent<EnemyHealth>() == null)
                {
                    Debug.LogError($"[Enemy] {group.EnemyPrefab.name} Prefab에 EnemyHealth가 없습니다.", group.EnemyPrefab);
                    isValid = false;
                }

                if (group.Count <= 0)
                {
                    Debug.LogError($"[Enemy] Spawn Group {i}의 Count가 올바르지 않습니다.", this);
                    isValid = false;
                }
            }

            return isValid;
        }

        private void HandleEnemyDied()
        {
            if (aliveEnemyCount <= 0)
                return;

            aliveEnemyCount--;

            if (aliveEnemyCount == 0)
                AllEnemiesDefeated?.Invoke();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                EnemyHealth enemyHealth = spawnedEnemies[i];

                if (enemyHealth != null)
                    enemyHealth.Died -= HandleEnemyDied;
            }
        }

        private void OnValidate()
        {
            minimumSpacing = Mathf.Max(0.1f, minimumSpacing);
            navMeshSampleDistance = Mathf.Max(0.1f, navMeshSampleDistance);
            maxPositionAttempts = Mathf.Max(1, maxPositionAttempts);
        }
    }
}