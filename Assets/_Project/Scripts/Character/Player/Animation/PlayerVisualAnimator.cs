using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerVisualAnimator : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField]
        private Transform body;

        [SerializeField]
        private Transform leftHand;

        [SerializeField]
        private Transform rightHand;

        [SerializeField]
        private Transform leftFoot;

        [SerializeField]
        private Transform rightFoot;

        [Header("State")]
        [SerializeField]
        [Min(0f)]
        private float movingSpeedThreshold = 0.15f;

        [SerializeField]
        [Min(0f)]
        private float runSpeedThreshold = 6f;

        [Header("Idle")]
        [SerializeField]
        [Min(0f)]
        private float idleCycleSpeed = 2f;

        [SerializeField]
        [Min(0f)]
        private float idleBodyBob = 0.025f;

        [SerializeField]
        [Min(0f)]
        private float idleHandBob = 0.015f;

        [Header("Walk")]
        [SerializeField]
        [Min(0f)]
        private float walkCycleSpeed = 7f;

        [SerializeField]
        [Min(0f)]
        private float walkStepDistance = 0.16f;

        [SerializeField]
        [Min(0f)]
        private float walkFootLift = 0.08f;

        [SerializeField]
        [Min(0f)]
        private float walkHandSwing = 0.13f;

        [SerializeField]
        [Min(0f)]
        private float walkBodyBob = 0.035f;

        [SerializeField]
        private float walkBodyLean = 2f;

        [Header("Run")]
        [SerializeField]
        [Min(0f)]
        private float runCycleSpeed = 10f;

        [SerializeField]
        [Min(0f)]
        private float runStepDistance = 0.28f;

        [SerializeField]
        [Min(0f)]
        private float runFootLift = 0.13f;

        [SerializeField]
        [Min(0f)]
        private float runHandSwing = 0.23f;

        [SerializeField]
        [Min(0f)]
        private float runBodyBob = 0.065f;

        [SerializeField]
        private float runBodyLean = 8f;

        [Header("Transition")]
        [SerializeField]
        [Min(0f)]
        private float transitionSpeed = 12f;

        private Vector3 bodyBasePosition;
        private Vector3 leftHandBasePosition;
        private Vector3 rightHandBasePosition;
        private Vector3 leftFootBasePosition;
        private Vector3 rightFootBasePosition;

        private Quaternion bodyBaseRotation;

        private float cycle;

        private bool isConfigured;

        private void Awake()
        {
            if (!ValidateParts())
            {
                return;
            }

            bodyBasePosition =
                body.localPosition;

            leftHandBasePosition =
                leftHand.localPosition;

            rightHandBasePosition =
                rightHand.localPosition;

            leftFootBasePosition =
                leftFoot.localPosition;

            rightFootBasePosition =
                rightFoot.localPosition;

            bodyBaseRotation =
                body.localRotation;

            isConfigured = true;
        }

        private bool ValidateParts()
        {
            if (body == null ||
                leftHand == null ||
                rightHand == null ||
                leftFoot == null ||
                rightFoot == null)
            {
                Debug.LogError(
                    "[Player] PlayerVisualAnimator의 캐릭터 파츠 참조가 누락되었습니다.");

                return false;
            }

            return true;
        }

        public void UpdateAnimation(
            float horizontalSpeed,
            bool isSprinting,
            float deltaTime)
        {
            if (!isConfigured)
            {
                return;
            }

            bool isMoving =
                horizontalSpeed > movingSpeedThreshold;

            bool isRunning =
                isMoving &&
                isSprinting &&
                horizontalSpeed > runSpeedThreshold;

            float cycleSpeed;

            if (!isMoving)
            {
                cycleSpeed = idleCycleSpeed;
            }
            else
            {
                cycleSpeed =
                    isRunning
                        ? runCycleSpeed
                        : walkCycleSpeed;
            }

            cycle += cycleSpeed * deltaTime;

            if (!isMoving)
            {
                UpdateIdle(deltaTime);
                return;
            }

            UpdateMovement(
                isRunning,
                deltaTime);
        }

        private void UpdateIdle(float deltaTime)
        {
            float bodyBob =
                Mathf.Sin(cycle) *
                idleBodyBob;

            float handBob =
                Mathf.Sin(cycle) *
                idleHandBob;

            float smoothFactor =
                GetSmoothFactor(deltaTime);

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.up * bodyBob;

            Vector3 leftHandTarget =
                leftHandBasePosition +
                Vector3.up * handBob;

            Vector3 rightHandTarget =
                rightHandBasePosition +
                Vector3.up * handBob;

            body.localPosition =
                Vector3.Lerp(
                    body.localPosition,
                    bodyTarget,
                    smoothFactor);

            body.localRotation =
                Quaternion.Slerp(
                    body.localRotation,
                    bodyBaseRotation,
                    smoothFactor);

            leftHand.localPosition =
                Vector3.Lerp(
                    leftHand.localPosition,
                    leftHandTarget,
                    smoothFactor);

            rightHand.localPosition =
                Vector3.Lerp(
                    rightHand.localPosition,
                    rightHandTarget,
                    smoothFactor);

            leftFoot.localPosition =
                Vector3.Lerp(
                    leftFoot.localPosition,
                    leftFootBasePosition,
                    smoothFactor);

            rightFoot.localPosition =
                Vector3.Lerp(
                    rightFoot.localPosition,
                    rightFootBasePosition,
                    smoothFactor);
        }

        private void UpdateMovement(
            bool isRunning,
            float deltaTime)
        {
            float stepDistance =
                isRunning
                    ? runStepDistance
                    : walkStepDistance;

            float footLift =
                isRunning
                    ? runFootLift
                    : walkFootLift;

            float handSwing =
                isRunning
                    ? runHandSwing
                    : walkHandSwing;

            float bodyBobAmount =
                isRunning
                    ? runBodyBob
                    : walkBodyBob;

            float bodyLean =
                isRunning
                    ? runBodyLean
                    : walkBodyLean;

            float swing =
                Mathf.Sin(cycle);

            float leftLift =
                Mathf.Max(0f, swing) *
                footLift;

            float rightLift =
                Mathf.Max(0f, -swing) *
                footLift;

            float bodyBob =
                (Mathf.Abs(Mathf.Cos(cycle)) - 0.5f) *
                2f *
                bodyBobAmount;

            Vector3 leftFootTarget =
                leftFootBasePosition +
                new Vector3(
                    0f,
                    leftLift,
                    swing * stepDistance);

            Vector3 rightFootTarget =
                rightFootBasePosition +
                new Vector3(
                    0f,
                    rightLift,
                    -swing * stepDistance);

            Vector3 leftHandTarget =
                leftHandBasePosition +
                Vector3.forward *
                (-swing * handSwing);

            Vector3 rightHandTarget =
                rightHandBasePosition +
                Vector3.forward *
                (swing * handSwing);

            Vector3 bodyTarget =
                bodyBasePosition +
                Vector3.up * bodyBob;

            Quaternion bodyRotationTarget =
                bodyBaseRotation *
                Quaternion.Euler(
                    bodyLean,
                    0f,
                    0f);

            float smoothFactor =
                GetSmoothFactor(deltaTime);

            body.localPosition =
                Vector3.Lerp(
                    body.localPosition,
                    bodyTarget,
                    smoothFactor);

            body.localRotation =
                Quaternion.Slerp(
                    body.localRotation,
                    bodyRotationTarget,
                    smoothFactor);

            leftHand.localPosition =
                Vector3.Lerp(
                    leftHand.localPosition,
                    leftHandTarget,
                    smoothFactor);

            rightHand.localPosition =
                Vector3.Lerp(
                    rightHand.localPosition,
                    rightHandTarget,
                    smoothFactor);

            leftFoot.localPosition =
                Vector3.Lerp(
                    leftFoot.localPosition,
                    leftFootTarget,
                    smoothFactor);

            rightFoot.localPosition =
                Vector3.Lerp(
                    rightFoot.localPosition,
                    rightFootTarget,
                    smoothFactor);
        }

        private float GetSmoothFactor(float deltaTime)
        {
            return 1f -
                Mathf.Exp(
                    -transitionSpeed *
                    deltaTime);
        }
    }
}