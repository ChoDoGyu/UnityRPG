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

        [Header("Death")]
        [SerializeField, Min(0.01f)] private float deathDuration = 0.7f;

        [Header("Transition")]
        [SerializeField, Min(0f)] private float transitionSpeed = 14f;

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
                Debug.LogError("[Boss] BossVisualAnimator의 Reference가 설정되지 않았습니다.", this);
                return;
            }

            bodyBasePosition = body.localPosition;
            bodyBaseRotation = body.localRotation;
            modelRootBaseScale = modelRoot.localScale;
            modelRootBaseRotation = modelRoot.localRotation;

            isConfigured = true;
        }

        public void UpdateAnimation(bool isMoving, BossPatternType patternType,
            BossPatternPhase patternPhase, float patternProgress, float deltaTime)
        {
            if (!isConfigured || isDead)
                return;

            modelRoot.localScale = modelRootBaseScale;

            if (patternType != BossPatternType.None)
            {
                UpdatePattern(patternType, patternPhase, patternProgress, deltaTime);
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
                return;

            isDead = true;
            StartCoroutine(DeathRoutine());
        }

        private void UpdatePattern(BossPatternType patternType, BossPatternPhase phase,
            float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            switch (patternType)
            {
                case BossPatternType.HeavySlash:
                    UpdateHeavySlash(phase, progress, deltaTime);
                    break;

                case BossPatternType.GroundSlam:
                    UpdateGroundSlam(phase, progress, deltaTime);
                    break;

                case BossPatternType.Charge:
                    UpdateCharge(phase, progress, deltaTime);
                    break;

                case BossPatternType.Shockwave:
                    UpdateShockwave(phase, progress, deltaTime);
                    break;
            }
        }

        private void UpdateHeavySlash(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position = bodyBasePosition + Vector3.back * 0.5f * progress;
                        Quaternion rotation = bodyBaseRotation *
                            Quaternion.Euler(-30f * progress, -55f * progress, -10f * progress);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.65f;
                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(25f, 65f, 15f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.45f * weight;
                        Quaternion rotation = bodyBaseRotation *
                            Quaternion.Euler(18f * weight, 45f * weight, 10f * weight);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateGroundSlam(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position = bodyBasePosition +
                            Vector3.back * 0.55f * progress +
                            Vector3.up * 0.8f * progress;

                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(-65f * progress, 0f, 0f);

                        modelRoot.localScale = modelRootBaseScale * (1f + 0.2f * progress);
                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.6f + Vector3.down * 0.5f;
                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(55f, 0f, 0f);

                        modelRoot.localScale = new Vector3(
                            modelRootBaseScale.x * 1.15f,
                            modelRootBaseScale.y * 0.85f,
                            modelRootBaseScale.z * 1.15f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;
                        Vector3 position = bodyBasePosition +
                            Vector3.forward * 0.45f * weight +
                            Vector3.down * 0.35f * weight;

                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(45f * weight, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateCharge(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position = bodyBasePosition +
                            Vector3.back * 0.45f * progress +
                            Vector3.down * 0.35f * progress;

                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(45f * progress, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.35f + Vector3.down * 0.25f;
                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(55f, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.4f * weight;
                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(35f * weight, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateShockwave(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position = bodyBasePosition +
                            Vector3.back * 0.4f * progress +
                            Vector3.up * 0.35f * progress;

                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(-35f * progress, 0f, 0f);

                        modelRoot.localScale = modelRootBaseScale * (1f + 0.12f * progress);
                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.7f;
                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(35f, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;
                        Vector3 position = bodyBasePosition + Vector3.forward * 0.5f * weight;
                        Quaternion rotation = bodyBaseRotation * Quaternion.Euler(25f * weight, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateIdle(float deltaTime)
        {
            cycle += idleCycleSpeed * deltaTime;

            float bob = Mathf.Sin(cycle) * idleBob;
            Vector3 position = bodyBasePosition + Vector3.up * bob;

            ApplyBodyPose(position, bodyBaseRotation, deltaTime);
        }

        private void UpdateMovement(float deltaTime)
        {
            cycle += moveCycleSpeed * deltaTime;

            float bob = Mathf.Abs(Mathf.Sin(cycle)) * moveBob;
            Vector3 position = bodyBasePosition + Vector3.up * bob;
            Quaternion rotation = bodyBaseRotation * Quaternion.Euler(moveLean, 0f, 0f);

            ApplyBodyPose(position, rotation, deltaTime);
        }

        private void ApplyBodyPose(Vector3 position, Quaternion rotation, float deltaTime)
        {
            float smoothFactor = 1f - Mathf.Exp(-transitionSpeed * deltaTime);

            body.localPosition = Vector3.Lerp(body.localPosition, position, smoothFactor);
            body.localRotation = Quaternion.Slerp(body.localRotation, rotation, smoothFactor);
        }

        private IEnumerator DeathRoutine()
        {
            float elapsedTime = 0f;
            Quaternion startRotation = modelRoot.localRotation;
            Quaternion targetRotation = modelRootBaseRotation * Quaternion.Euler(0f, 0f, 90f);

            while (elapsedTime < deathDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / deathDuration);

                modelRoot.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);
                yield return null;
            }

            modelRoot.localRotation = targetRotation;
        }
    }
}