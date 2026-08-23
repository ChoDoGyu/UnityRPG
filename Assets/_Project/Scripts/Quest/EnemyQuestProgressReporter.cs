using UnityEngine;
using UnityRPG.AI;

namespace UnityRPG.Quest
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyQuestProgressReporter : MonoBehaviour
    {
        private EnemyContext enemyContext;
        private EnemyHealth enemyHealth;

        private void Awake()
        {
            enemyContext = GetComponent<EnemyContext>();
            enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            if (enemyHealth != null)
                enemyHealth.DiedBy += HandleDiedBy;
        }

        private void OnDisable()
        {
            if (enemyHealth != null)
                enemyHealth.DiedBy -= HandleDiedBy;
        }

        private void HandleDiedBy(GameObject source)
        {
            if (source == null || !enemyContext.IsConfigured)
                return;

            PlayerQuestLog questLog = source.GetComponentInParent<PlayerQuestLog>();

            if (questLog == null)
                return;

            string enemyId = enemyContext.Definition.EnemyId;

            if (string.IsNullOrWhiteSpace(enemyId))
                return;

            questLog.AddProgress(QuestObjectiveType.DefeatEnemy, enemyId);
        }
    }
}