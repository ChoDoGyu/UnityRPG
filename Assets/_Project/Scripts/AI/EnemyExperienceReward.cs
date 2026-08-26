using UnityEngine;
using UnityRPG.Character.Growth;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyExperienceReward : MonoBehaviour
    {
        private EnemyContext context;
        private EnemyHealth enemyHealth;
        private bool hasRewarded;

        private void Awake()
        {
            context = GetComponent<EnemyContext>();
            enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            enemyHealth.DiedBy += HandleDiedBy;
        }

        private void OnDisable()
        {
            enemyHealth.DiedBy -= HandleDiedBy;
        }

        private void HandleDiedBy(GameObject source)
        {
            if (hasRewarded || source == null || !context.IsConfigured)
                return;

            PlayerGrowth playerGrowth = source.GetComponentInParent<PlayerGrowth>();

            if (playerGrowth == null)
                return;

            int experienceReward = context.Definition.ExperienceReward;

            if (experienceReward <= 0)
                return;

            hasRewarded = true;
            playerGrowth.AddExperience(experienceReward);
        }
    }
}