using UnityEngine;
using UnityRPG.Character.Stats;
using UnityRPG.Combat;
using UnityRPG.Core;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerProjectileSkill : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private Transform directionReference;

        [SerializeField]
        private SkillProjectile projectilePrefab;

        [Header("Projectile")]
        [SerializeField]
        [Min(0.01f)]
        private float projectileSpeed = 12f;

        [SerializeField]
        [Min(0.01f)]
        private float projectileLifetime = 2.5f;

        [SerializeField]
        [Min(0f)]
        private float hitRadius = 0.2f;

        [SerializeField]
        private LayerMask collisionMask;

        [Header("Damage")]
        [SerializeField]
        [Min(0f)]
        private float damageMultiplier = 2f;

        [Header("Action")]
        [SerializeField]
        [Min(0.01f)]
        private float actionDuration = 0.3f;

        [Header("SFX")]
        [SerializeField] private AudioClip projectileSfx;

        public float ActionDuration => actionDuration;

        private bool isConfigured;

        private PlayerStats playerStats;

        private void Awake()
        {
            if (spawnPoint == null || directionReference == null || projectilePrefab == null)
            {
                Debug.LogError("[Skill] Projectile Skill의 Reference가 설정되지 않았습니다.", this);

                return;
            }

            playerStats = GetComponent<PlayerStats>();

            isConfigured = true;
        }

        public bool TryStart()
        {
            if (!isConfigured || playerStats == null || !playerStats.IsConfigured)
            {
                return false;
            }

            Vector3 direction = directionReference.forward;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return false;
            }

            direction.Normalize();

            SkillProjectile projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(direction));

            DamageInfo damageInfo =
                DamageCalculator.CreateAttackDamage(playerStats.Attack, damageMultiplier, playerStats.CritChance, playerStats.CritDamage, gameObject);

            projectile.Initialize(direction, projectileSpeed, projectileLifetime, hitRadius, collisionMask, damageInfo);

            AudioService.Instance?.PlaySfx(projectileSfx);

            return true;
        }
    }
}