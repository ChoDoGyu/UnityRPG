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

        [Header("Attack")]
        [SerializeField]
        [Min(0.01f)]
        private float attackVisualDuration = 0.25f;

        [Header("Transition")]
        [SerializeField]
        [Min(0f)]
        private float transitionSpeed = 12f;

        private Vector3 bodyBasePosition;
        private Quaternion bodyBaseRotation;
        private Vector3 modelRootBaseScale;

        private float cycle;

        private float remainingAttackVisualDuration;
        private EnemyType currentAttackType;

        private bool isConfigured;

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

            isConfigured = true;
        }

        public void UpdateAnimation(bool isMoving, float deltaTime)
        {
            if (!isConfigured)
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
    }
}