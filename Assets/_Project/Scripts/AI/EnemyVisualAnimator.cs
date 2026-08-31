using System.Collections;
using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemyVisualAnimator : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Transform modelRoot;
        [SerializeField] private Transform body;

        [Header("Idle")]
        [SerializeField, Min(0f)] private float idleCycleSpeed = 2f;
        [SerializeField, Min(0f)] private float idleBob = 0.025f;

        [Header("Move")]
        [SerializeField, Min(0f)] private float moveCycleSpeed = 7f;
        [SerializeField, Min(0f)] private float moveBob = 0.05f;
        [SerializeField] private float moveLean = 6f;

        [Header("Melee Attack")]
        [SerializeField, Min(0f)] private float meleeWindupBack = 0.35f;
        [SerializeField, Min(0f)] private float meleeWindupDown = 0.18f;
        [SerializeField] private float meleeWindupPitch = -20f;
        [SerializeField, Min(0f)] private float meleeAttackForward = 0.38f;
        [SerializeField, Min(0f)] private float meleeAttackUp = 0.22f;
        [SerializeField] private float meleeAttackPitch = 28f;

        [Header("Ranged Attack")]
        [SerializeField, Min(0f)] private float rangedWindupForward = 0.18f;
        [SerializeField, Min(0f)] private float rangedWindupDown = 0.12f;
        [SerializeField] private float rangedWindupPitch = 18f;
        [SerializeField, Range(0f, 0.5f)] private float rangedWindupScale = 0.1f;
        [SerializeField, Min(0f)] private float rangedRecoveryBack = 0.18f;
        [SerializeField] private float rangedRecoveryPitch = -10f;
        [SerializeField, Range(0f, 0.5f)] private float rangedRecoveryScale = 0.05f;

        [Header("Elite Slam")]
        [SerializeField, Min(0f)] private float slamWindupBack = 0.45f;
        [SerializeField, Min(0f)] private float slamWindupUp = 0.55f;
        [SerializeField] private float slamWindupPitch = -55f;
        [SerializeField, Range(0f, 0.5f)] private float slamWindupScale = 0.15f;
        [SerializeField, Min(0f)] private float slamRecoveryForward = 0.5f;
        [SerializeField, Min(0f)] private float slamRecoveryDown = 0.4f;
        [SerializeField] private float slamRecoveryPitch = 45f;
        [SerializeField, Min(0.1f)] private float slamImpactWidthScale = 1.1f;
        [SerializeField, Min(0.1f)] private float slamImpactHeightScale = 0.9f;

        [Header("Death")]
        [SerializeField, Min(0.01f)] private float deathDuration = 0.4f;

        [Header("Transition")]
        [SerializeField, Min(0f)] private float transitionSpeed = 12f;

        private EnemyVisualPose pose;
        private EnemyLocomotionVisual locomotionVisual;
        private EnemyAttackVisual attackVisual;
        private EliteSlamVisual slamVisual;

        private bool isConfigured;
        private bool isDead;

        private void Awake()
        {
            if (!ValidateReferences())
                return;

            pose = new EnemyVisualPose(modelRoot, body);

            CreateVisualModules();

            isConfigured = true;
        }

        public void UpdateAnimation(
            bool isMoving,
            EnemyAttackPhase attackPhase,
            float attackProgress,
            bool isSlamAttack,
            EnemyType enemyType,
            float deltaTime)
        {
            if (!isConfigured || isDead)
                return;

            pose.ResetModelRoot();

            if (attackPhase == EnemyAttackPhase.Windup)
            {
                if (isSlamAttack)
                    slamVisual.UpdateWindup(attackProgress, deltaTime);
                else
                    attackVisual.UpdateWindup(enemyType, attackProgress, deltaTime);

                return;
            }

            if (attackPhase == EnemyAttackPhase.Recovery)
            {
                if (isSlamAttack)
                    slamVisual.UpdateRecovery(attackProgress, deltaTime);
                else
                    attackVisual.UpdateRecovery(enemyType, attackProgress, deltaTime);

                return;
            }

            if (isMoving)
            {
                locomotionVisual.UpdateMovement(deltaTime);
                return;
            }

            locomotionVisual.UpdateIdle(deltaTime);
        }

        public void PlayDeath()
        {
            if (!isConfigured || isDead)
                return;

            isDead = true;
            StartCoroutine(DeathRoutine());
        }

        private bool ValidateReferences()
        {
            if (modelRoot != null && body != null)
                return true;

            Debug.LogError("[Enemy] EnemyVisualAnimator의 Reference가 설정되지 않았습니다.", this);
            return false;
        }

        private void CreateVisualModules()
        {
            locomotionVisual = new EnemyLocomotionVisual(
                pose,
                idleCycleSpeed,
                idleBob,
                moveCycleSpeed,
                moveBob,
                moveLean,
                transitionSpeed);

            EnemyMeleeVisualSettings meleeSettings = new EnemyMeleeVisualSettings(
                meleeWindupBack,
                meleeWindupDown,
                meleeWindupPitch,
                meleeAttackForward,
                meleeAttackUp,
                meleeAttackPitch);

            EnemyRangedVisualSettings rangedSettings = new EnemyRangedVisualSettings(
                rangedWindupForward,
                rangedWindupDown,
                rangedWindupPitch,
                rangedWindupScale,
                rangedRecoveryBack,
                rangedRecoveryPitch,
                rangedRecoveryScale);

            attackVisual = new EnemyAttackVisual(
                pose,
                transitionSpeed,
                meleeSettings,
                rangedSettings);

            slamVisual = new EliteSlamVisual(
                pose,
                transitionSpeed,
                slamWindupBack,
                slamWindupUp,
                slamWindupPitch,
                slamWindupScale,
                slamRecoveryForward,
                slamRecoveryDown,
                slamRecoveryPitch,
                slamImpactWidthScale,
                slamImpactHeightScale);
        }

        private IEnumerator DeathRoutine()
        {
            float elapsedTime = 0f;

            Quaternion startRotation = pose.ModelRoot.localRotation;
            Quaternion targetRotation =
                pose.ModelRootBaseRotation * Quaternion.Euler(0f, 0f, 90f);

            while (elapsedTime < deathDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / deathDuration);

                pose.ModelRoot.localRotation =
                    Quaternion.Slerp(startRotation, targetRotation, progress);

                yield return null;
            }

            pose.ModelRoot.localRotation = targetRotation;
        }
    }
}