using System.Collections.Generic;
using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.Character.Stats;
using UnityRPG.VFX;
using UnityRPG.Core;

namespace UnityRPG.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(CombatVfxController))]
    public sealed class PlayerDashSlashSkill : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Transform directionReference;

        [Header("Movement")]
        [SerializeField, Min(0.01f)] private float dashDistance = 4f;
        [SerializeField, Min(0.01f)] private float dashDuration = 0.25f;

        [Header("Hit")]
        [SerializeField, Min(0f)] private float hitRadius = 0.8f;
        [SerializeField] private LayerMask targetLayer;

        [Header("Damage")]
        [SerializeField, Min(0f)] private float damageMultiplier = 1.5f;

        [Header("SFX")]
        [SerializeField] private AudioClip dashSlashSfx;

        private readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        private readonly Collider[] hitBuffer = new Collider[32];

        private CharacterController characterController;
        private PlayerStats playerStats;
        private CombatVfxController combatVfxController;

        private Vector3 dashDirection;
        private float remainingDuration;
        private float dashSpeed;
        private bool isConfigured;

        public bool IsActive => remainingDuration > 0f;
        public float ActionDuration => dashDuration;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerStats = GetComponent<PlayerStats>();
            combatVfxController = GetComponent<CombatVfxController>();

            if (directionReference == null)
            {
                Debug.LogError("[Skill] DashSlash의 Direction Reference가 설정되지 않았습니다.", this);
                return;
            }

            isConfigured = true;
        }

        public bool TryStart()
        {
            if (!isConfigured || playerStats == null || !playerStats.IsConfigured)
                return false;

            if (IsActive)
                return false;

            dashDirection = directionReference.forward;
            dashDirection.y = 0f;

            if (dashDirection.sqrMagnitude <= 0.001f)
                return false;

            dashDirection.Normalize();
            dashSpeed = dashDistance / dashDuration;
            remainingDuration = dashDuration;

            hitTargets.Clear();

            combatVfxController.PlayDashSlash();
            AudioService.Instance?.PlaySfx(dashSlashSfx);

            CheckHitTargets();

            return true;
        }

        public void UpdateSkill(float deltaTime)
        {
            if (!IsActive)
                return;

            float moveTime = Mathf.Min(deltaTime, remainingDuration);

            characterController.Move(dashDirection * dashSpeed * moveTime);
            CheckHitTargets();

            remainingDuration = Mathf.Max(0f, remainingDuration - moveTime);
        }

        private void CheckHitTargets()
        {
            Vector3 center = transform.TransformPoint(characterController.center);

            int hitCount = Physics.OverlapSphereNonAlloc(
                center,
                hitRadius,
                hitBuffer,
                targetLayer,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = hitBuffer[i];
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();

                if (damageable == null || !hitTargets.Add(damageable))
                    continue;

                DamageInfo damageInfo = DamageCalculator.CreateAttackDamage(
                    playerStats.Attack,
                    damageMultiplier,
                    playerStats.CritChance,
                    playerStats.CritDamage,
                    gameObject);

                damageable.TakeDamage(damageInfo);

                Vector3 hitPoint = hit.ClosestPoint(transform.position);
                combatVfxController.PlayHit(hitPoint);
            }
        }
    }
}