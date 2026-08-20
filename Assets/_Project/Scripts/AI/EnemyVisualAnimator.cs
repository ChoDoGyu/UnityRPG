using System.Collections;
using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemyVisualAnimator : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private Transform modelRoot;

        [SerializeField]
        private Transform body;

        [Header("Idle")]
        [SerializeField]
        [Min(0f)]
        private float idleCycleSpeed = 2f;

        [SerializeField]
        [Min(0f)]
        private float idleBob = 0.025f;

        [Header("Move")]
        [SerializeField]
        [Min(0f)]
        private float moveCycleSpeed = 7f;

        [SerializeField]
        [Min(0f)]
        private float moveBob = 0.05f;

        [SerializeField]
        private float moveLean = 6f;

        [Header("Death")]
        [SerializeField]
        [Min(0.01f)]
        private float deathDuration = 0.4f;

        [Header("Transition")]
        [SerializeField]
        [Min(0f)]
        private float transitionSpeed = 12f;

        private Vector3 bodyBasePosition;
        private Quaternion bodyBaseRotation;

        private Vector3 modelRootBaseScale;
        private Quaternion modelRootBaseRotation;

        private float cycle;

        private bool isConfigured;
        private bool isDead;

        private void Awake()
        {
            if (modelRoot == null || body == null)
            {
                Debug.LogError(
                    "[Enemy] EnemyVisualAnimator의 Reference가 설정되지 않았습니다.",
                    this);

                return;
            }

            bodyBasePosition = body.localPosition;
            bodyBaseRotation = body.localRotation;

            modelRootBaseScale = modelRoot.localScale;
            modelRootBaseRotation = modelRoot.localRotation;

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
            {
                return;
            }

            modelRoot.localScale = modelRootBaseScale;

            if (attackPhase == EnemyAttackPhase.Windup)
            {
                if (isSlamAttack)
                {
                    UpdateSlamWindup(attackProgress, deltaTime);
                }
                else
                {
                    UpdateWindup(enemyType, attackProgress, deltaTime);
                }

                return;
            }

            if (attackPhase == EnemyAttackPhase.Recovery)
            {
                if (isSlamAttack)
                {
                    UpdateSlamRecovery(attackProgress, deltaTime);
                }
                else
                {
                    UpdateRecovery(enemyType, attackProgress, deltaTime);
                }

                return;
            }

            if (isMoving)
            {
                UpdateMovement(deltaTime);
                return;
            }

            UpdateIdle(deltaTime);
        }

        public void PlayDeath()
        {
            if (!isConfigured || isDead)
            {
                return;
            }

            isDead = true;
            StartCoroutine(DeathRoutine());
        }

        private void UpdateWindup(
            EnemyType enemyType,
            float progress,
            float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            switch (enemyType)
            {
                case EnemyType.Melee:
                    UpdateMeleeWindup(progress, deltaTime);
                    break;

                case EnemyType.Ranged:
                    UpdateRangedWindup(progress, deltaTime);
                    break;
            }
        }

        private void UpdateRecovery(
            EnemyType enemyType,
            float progress,
            float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            switch (enemyType)
            {
                case EnemyType.Melee:
                    UpdateMeleeRecovery(progress, deltaTime);
                    break;

                case EnemyType.Ranged:
                    UpdateRangedRecovery(progress, deltaTime);
                    break;
            }
        }

        private void UpdateMeleeWindup(float progress, float deltaTime)
        {
            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.back * 0.4f * progress;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(-25f * progress, 0f, 0f);

            ApplyBodyPose(bodyTarget, rotationTarget, deltaTime);
        }

        private void UpdateMeleeRecovery(float progress, float deltaTime)
        {
            float weight = 1f - progress;

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.forward * 0.35f * weight;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(18f * weight, 0f, 0f);

            ApplyBodyPose(bodyTarget, rotationTarget, deltaTime);
        }

        private void UpdateRangedWindup(float progress, float deltaTime)
        {
            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.forward * 0.18f * progress +
                Vector3.down * 0.12f * progress;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(18f * progress, 0f, 0f);

            float scale = 1f - 0.1f * progress;
            modelRoot.localScale = modelRootBaseScale * scale;

            ApplyBodyPose(bodyTarget, rotationTarget, deltaTime);
        }

        private void UpdateRangedRecovery(float progress, float deltaTime)
        {
            float weight = 1f - progress;

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.back * 0.18f * weight;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(-10f * weight, 0f, 0f);

            float scale = 1f + 0.05f * weight;
            modelRoot.localScale = modelRootBaseScale * scale;

            ApplyBodyPose(bodyTarget, rotationTarget, deltaTime);
        }

        private void UpdateIdle(float deltaTime)
        {
            cycle += idleCycleSpeed * deltaTime;

            float bob = Mathf.Sin(cycle) * idleBob;

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.up * bob;

            float smoothFactor = GetSmoothFactor(deltaTime);

            body.localPosition = Vector3.Lerp(
                body.localPosition,
                bodyTarget,
                smoothFactor);

            body.localRotation = Quaternion.Slerp(
                body.localRotation,
                bodyBaseRotation,
                smoothFactor);
        }

        private void UpdateMovement(float deltaTime)
        {
            cycle += moveCycleSpeed * deltaTime;

            float bob = Mathf.Abs(Mathf.Sin(cycle)) * moveBob;

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.up * bob;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(moveLean, 0f, 0f);

            float smoothFactor = GetSmoothFactor(deltaTime);

            body.localPosition = Vector3.Lerp(
                body.localPosition,
                bodyTarget,
                smoothFactor);

            body.localRotation = Quaternion.Slerp(
                body.localRotation,
                rotationTarget,
                smoothFactor);
        }

        private void ApplyBodyPose(
            Vector3 bodyTarget,
            Quaternion rotationTarget,
            float deltaTime)
        {
            float smoothFactor = GetSmoothFactor(deltaTime);

            body.localPosition = Vector3.Lerp(
                body.localPosition,
                bodyTarget,
                smoothFactor);

            body.localRotation = Quaternion.Slerp(
                body.localRotation,
                rotationTarget,
                smoothFactor);
        }

        private float GetSmoothFactor(float deltaTime)
        {
            return 1f - Mathf.Exp(-transitionSpeed * deltaTime);
        }

        private IEnumerator DeathRoutine()
        {
            float elapsedTime = 0f;

            Quaternion startRotation = modelRoot.localRotation;
            Quaternion targetRotation =
                modelRootBaseRotation *
                Quaternion.Euler(0f, 0f, 90f);

            while (elapsedTime < deathDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress =
                    Mathf.Clamp01(elapsedTime / deathDuration);

                modelRoot.localRotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    progress);

                yield return null;
            }

            modelRoot.localRotation = targetRotation;
        }

        private void UpdateSlamWindup(float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.back * 0.45f * progress +
                Vector3.up * 0.55f * progress;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(-55f * progress, 0f, 0f);

            float scale = 1f + 0.15f * progress;
            modelRoot.localScale = modelRootBaseScale * scale;

            ApplyBodyPose(bodyTarget, rotationTarget, deltaTime);
        }

        private void UpdateSlamRecovery(float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            float weight = 1f - progress;

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.forward * 0.5f * weight +
                Vector3.down * 0.4f * weight;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(45f * weight, 0f, 0f);

            Vector3 impactScale = new Vector3(
                modelRootBaseScale.x * 1.1f,
                modelRootBaseScale.y * 0.9f,
                modelRootBaseScale.z * 1.1f);

            modelRoot.localScale = Vector3.Lerp(
                modelRootBaseScale,
                impactScale,
                weight);

            ApplyBodyPose(bodyTarget, rotationTarget, deltaTime);
        }
    }
}