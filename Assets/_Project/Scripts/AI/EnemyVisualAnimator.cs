using UnityEngine;
using System.Collections;

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

        [Header("Attack")]
        [SerializeField]
        [Min(0.01f)]
        private float attackVisualDuration = 0.25f;

        [Header("Transition")]
        [SerializeField]
        [Min(0f)]
        private float transitionSpeed = 12f;

        [Header("Death")]
        [SerializeField]
        [Min(0.01f)]
        private float deathDuration = 0.4f;

        private Vector3 bodyBasePosition;
        private Vector3 modelRootBaseScale;

        private Quaternion bodyBaseRotation;
        private Quaternion modelRootBaseRotation;
        

        private float cycle;
        private float remainingAttackVisualDuration;

        private EnemyType currentAttackType;

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

        public void UpdateAnimation(bool isMoving, float deltaTime)
        {
            if (!isConfigured || isDead)
            {
                return;
            }

            modelRoot.localScale = modelRootBaseScale;

            if (remainingAttackVisualDuration > 0f)
            {
                UpdateAttack(deltaTime);
                return;
            }

            if (isMoving)
            {
                UpdateMovement(deltaTime);
                return;
            }

            UpdateIdle(deltaTime);
        }

        public void PlayAttack(EnemyType enemyType)
        {
            if (!isConfigured)
            {
                return;
            }

            currentAttackType = enemyType;
            remainingAttackVisualDuration = attackVisualDuration;
        }

        private void UpdateIdle(float deltaTime)
        {
            cycle += idleCycleSpeed * deltaTime;

            float bob = Mathf.Sin(cycle) * idleBob;
            Vector3 bodyTarget = bodyBasePosition + Vector3.up * bob;
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

            Vector3 bodyTarget = bodyBasePosition + Vector3.up * bob;

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

        private void UpdateAttack(float deltaTime)
        {
            remainingAttackVisualDuration = Mathf.Max(
                0f,
                remainingAttackVisualDuration - deltaTime);

            float progress =
                1f -
                remainingAttackVisualDuration /
                attackVisualDuration;

            float weight = Mathf.Sin(progress * Mathf.PI);

            switch (currentAttackType)
            {
                case EnemyType.Melee:
                    UpdateMeleeAttack(weight, deltaTime);
                    break;

                case EnemyType.Ranged:
                    UpdateRangedAttack(weight, deltaTime);
                    break;
            }
        }

        private void UpdateMeleeAttack(float weight, float deltaTime)
        {
            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.forward * 0.35f * weight;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(18f * weight, 0f, 0f);

            ApplyAttackPose(
                bodyTarget,
                rotationTarget,
                deltaTime);
        }

        private void UpdateRangedAttack(float weight, float deltaTime)
        {
            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.back * 0.15f * weight;

            Quaternion rotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(-8f * weight, 0f, 0f);

            float pulse = 1f + 0.05f * weight;
            modelRoot.localScale = modelRootBaseScale * pulse;

            ApplyAttackPose(
                bodyTarget,
                rotationTarget,
                deltaTime);
        }

        private void ApplyAttackPose(
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

        public void PlayDeath()
        {
            if (!isConfigured || isDead)
            {
                return;
            }

            isDead = true;
            remainingAttackVisualDuration = 0f;

            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            float elapsedTime = 0f;

            Quaternion startRotation = modelRoot.localRotation;
            Quaternion targetRotation =
                modelRootBaseRotation * Quaternion.Euler(0f, 0f, 90f);

            while (elapsedTime < deathDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / deathDuration);

                modelRoot.localRotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    progress);

                yield return null;
            }

            modelRoot.localRotation = targetRotation;
        }
    }
}