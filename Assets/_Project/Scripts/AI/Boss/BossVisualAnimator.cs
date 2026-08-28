using System.Collections;
using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class BossVisualAnimator : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Transform modelRoot;
        [SerializeField] private Transform body;

        [Header("Idle")]
        [SerializeField, Min(0f)] private float idleCycleSpeed = 1.5f;
        [SerializeField, Min(0f)] private float idleBob = 0.04f;

        [Header("Move")]
        [SerializeField, Min(0f)] private float moveCycleSpeed = 5f;
        [SerializeField, Min(0f)] private float moveBob = 0.07f;
        [SerializeField] private float moveLean = 8f;

        [Header("Heavy Slash")]
        [SerializeField, Min(0f)] private float heavyWindupBack = 0.5f;
        [SerializeField] private Vector3 heavyWindupRotation = new Vector3(-30f, -55f, -10f);

        [SerializeField, Min(0f)] private float heavyActiveForward = 0.65f;
        [SerializeField] private Vector3 heavyActiveRotation = new Vector3(25f, 65f, 15f);

        [SerializeField, Min(0f)] private float heavyRecoveryForward = 0.45f;
        [SerializeField] private Vector3 heavyRecoveryRotation = new Vector3(18f, 45f, 10f);

        [Header("Ground Slam")]
        [SerializeField, Min(0f)] private float slamWindupBack = 0.55f;
        [SerializeField, Min(0f)] private float slamWindupUp = 0.8f;
        [SerializeField] private float slamWindupPitch = -65f;
        [SerializeField, Range(0f, 0.5f)] private float slamWindupScale = 0.2f;

        [SerializeField, Min(0f)] private float slamActiveForward = 0.6f;
        [SerializeField, Min(0f)] private float slamActiveDown = 0.5f;
        [SerializeField] private float slamActivePitch = 55f;
        [SerializeField, Min(0.1f)] private float slamImpactWidthScale = 1.15f;
        [SerializeField, Min(0.1f)] private float slamImpactHeightScale = 0.85f;

        [SerializeField, Min(0f)] private float slamRecoveryForward = 0.45f;
        [SerializeField, Min(0f)] private float slamRecoveryDown = 0.35f;
        [SerializeField] private float slamRecoveryPitch = 45f;

        [Header("Charge")]
        [SerializeField, Min(0f)] private float chargeWindupBack = 0.45f;
        [SerializeField, Min(0f)] private float chargeWindupDown = 0.35f;
        [SerializeField] private float chargeWindupPitch = 45f;

        [SerializeField, Min(0f)] private float chargeActiveForward = 0.35f;
        [SerializeField, Min(0f)] private float chargeActiveDown = 0.25f;
        [SerializeField] private float chargeActivePitch = 55f;

        [SerializeField, Min(0f)] private float chargeRecoveryForward = 0.4f;
        [SerializeField] private float chargeRecoveryPitch = 35f;

        [Header("Shockwave")]
        [SerializeField, Min(0f)] private float shockwaveWindupBack = 0.4f;
        [SerializeField, Min(0f)] private float shockwaveWindupUp = 0.35f;
        [SerializeField] private float shockwaveWindupPitch = -35f;
        [SerializeField, Range(0f, 0.5f)] private float shockwaveWindupScale = 0.12f;

        [SerializeField, Min(0f)] private float shockwaveActiveForward = 0.7f;
        [SerializeField] private float shockwaveActivePitch = 35f;

        [SerializeField, Min(0f)] private float shockwaveRecoveryForward = 0.5f;
        [SerializeField] private float shockwaveRecoveryPitch = 25f;

        [Header("Death")]
        [SerializeField, Min(0.01f)] private float deathDuration = 0.7f;

        [Header("Transition")]
        [SerializeField, Min(0f)] private float transitionSpeed = 14f;

        private BossVisualPose pose;
        private BossLocomotionVisual locomotionVisual;
        private BossPatternVisual patternVisual;

        private bool isConfigured;
        private bool isDead;

        private void Awake()
        {
            if (!ValidateReferences())
                return;

            pose = new BossVisualPose(modelRoot, body);

            CreateVisualModules();

            isConfigured = true;
        }

        public void UpdateAnimation(
            bool isMoving,
            BossPatternType patternType,
            BossPatternPhase patternPhase,
            float patternProgress,
            float deltaTime)
        {
            if (!isConfigured || isDead)
                return;

            pose.ResetModelRoot();

            if (patternType != BossPatternType.None)
            {
                patternVisual.UpdatePattern(patternType, patternPhase, patternProgress, deltaTime);
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

            Debug.LogError("[Boss] BossVisualAnimator의 Reference가 설정되지 않았습니다.", this);
            return false;
        }

        private void CreateVisualModules()
        {
            locomotionVisual = new BossLocomotionVisual(
                pose,
                idleCycleSpeed,
                idleBob,
                moveCycleSpeed,
                moveBob,
                moveLean,
                transitionSpeed);

            BossHeavySlashVisualSettings heavySlashSettings =
                new BossHeavySlashVisualSettings(
                    heavyWindupBack,
                    heavyWindupRotation,
                    heavyActiveForward,
                    heavyActiveRotation,
                    heavyRecoveryForward,
                    heavyRecoveryRotation);

            BossGroundSlamVisualSettings groundSlamSettings =
                new BossGroundSlamVisualSettings(
                    slamWindupBack,
                    slamWindupUp,
                    slamWindupPitch,
                    slamWindupScale,
                    slamActiveForward,
                    slamActiveDown,
                    slamActivePitch,
                    slamImpactWidthScale,
                    slamImpactHeightScale,
                    slamRecoveryForward,
                    slamRecoveryDown,
                    slamRecoveryPitch);

            BossChargeVisualSettings chargeSettings =
                new BossChargeVisualSettings(
                    chargeWindupBack,
                    chargeWindupDown,
                    chargeWindupPitch,
                    chargeActiveForward,
                    chargeActiveDown,
                    chargeActivePitch,
                    chargeRecoveryForward,
                    chargeRecoveryPitch);

            BossShockwaveVisualSettings shockwaveSettings =
                new BossShockwaveVisualSettings(
                    shockwaveWindupBack,
                    shockwaveWindupUp,
                    shockwaveWindupPitch,
                    shockwaveWindupScale,
                    shockwaveActiveForward,
                    shockwaveActivePitch,
                    shockwaveRecoveryForward,
                    shockwaveRecoveryPitch);

            patternVisual = new BossPatternVisual(
                pose,
                transitionSpeed,
                heavySlashSettings,
                groundSlamSettings,
                chargeSettings,
                shockwaveSettings);
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